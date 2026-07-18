# KGSystem — Business Workflow & Data Verification Review

This document describes the kindergarten's operational workflow as implemented today, and reports the results of an end-to-end verification pass: real data was created through the live system, dashboard/report numbers were independently recomputed by hand, and the two were compared.

**Update:** all 6 findings below (including one uncovered mid-fix) have now been fixed and re-verified live against the running app — see the "Fixed & verified" note on each.

## 1. Business workflow (end to end)

### Roles
Two staff roles, enforced on both the API and the UI navigation:
- **Manager** — full access: dashboard, reference-data setup (phases, academic years, monthly fees), branding/settings, and can register new staff accounts.
- **Accountant** — day-to-day operations: children, enrollments, attendance, payments. Cannot touch reference data, dashboard, or settings.

### One-time / yearly setup (Manager)
1. Define **KG Phases** (e.g. KG1, KG2) — the levels the nursery offers.
2. Define an **Academic Year** (e.g. 2025-2026) and mark it active — only one year should be active at a time; reports and dashboards scope themselves to whichever year is flagged active.
3. Define **Monthly Fees** for that year — one fee amount + due date per calendar month.
4. Set **Branding** (app name AR/EN, logo, colors, currency) — shown across the UI and on printed reports.

### Per-child lifecycle (Accountant, day to day)
1. **Register the child** — bilingual name, date of birth, gender, guardian contact info.
2. **Enroll** the child into a KG Phase for the active Academic Year.
3. **Daily attendance** — mark Present / Absent / Excused per child, per day (single entry or a batch save for the whole class at once).
4. **Monthly payment** — record what's due and what's been paid for a given month; partial payments and discounts are supported; status (Unpaid/Partial/Paid) is derived automatically.
5. **Withdrawal** — if a child leaves, the record can be deleted from the Children screen.

### Reporting
- **Dashboard** (Manager) — today's snapshot: total/active students, today's absences and absence rate, today's and this month's income, pending payments, and breakdowns by phase and payment status.
- **Attendance Reports** — a printable monthly register (one row per child, one column per day) and a per-child attendance history report, both exportable to PDF.

## 2. Dashboard metrics — what each number means

| Metric | Computed as |
|---|---|
| Total / Active Students | Count of all children / children with Status = Active |
| Absent Today | Attendance records dated today with Status = Absent |
| Absent Rate | Absent Today ÷ **Active** Students × 100 |
| Today's Income | Sum of `AmountPaid` for payments with `PaidDate` = today **and** Status = Paid |
| Monthly Income | Sum of `AmountPaid` for payments with `PaidDate` in the current calendar month, regardless of status (so a Partial payment's received amount is correctly included) |
| Pending Payments (count / amount) | Count and `AmountDue − AmountPaid − Discount` sum, across payments with Status Unpaid or Partial |
| Phase Distribution / Enrollment Trends | Scoped to enrollments in the **active** academic year only |
| Payment Status Distribution / Recent Payments | Across **all** payments ever recorded (not scoped to the active year) |

## 3. How this was verified

The running app (frontend + API + a local SQL Server instance) was driven directly:
1. Captured the existing seed data's dashboard numbers as a baseline and hand-verified every figure against the underlying children/payments/enrollments (all correct — see §5).
2. Created a new child, enrollment, attendance record, and payment through the API/UI, with values chosen to test edge cases (a discount applied at payment creation; a child later deleted).
3. Re-fetched the dashboard and reports after each step and compared against hand-calculated expected values.
4. Cleaned up all test data afterward (confirmed the dashboard returned to the exact original baseline).

## 4. Findings

### Finding 1 — Critical: Deleting a child does not clean up their enrollment, attendance, or payment records
**Fixed & verified live.**

`DeleteChildCommandHandler` (backend) only removed the `Child` entity. The app uses soft-delete (an `IsDeleted` flag set via a global `SaveChanges` interceptor) — but nothing cascaded that flag to the child's `Enrollment`, `Attendance`, or `Payment` rows. They stayed fully "active" in the database after the child was gone.

Verified impact before the fix:
- The deleted child's **Attendance** record kept counting in "Absent Today" / "Absent Rate" — permanently, since nothing ever cleaned it up.
- The deleted child's **Payment** kept counting in `pendingPaymentsCount` (a query that only checks the Payment's own status, no join) — while `pendingPaymentsAmount` happened to *look* right, but only by accident (its query joins through Enrollment → Child, and the child's own soft-delete filter incidentally hid the row from that one query, not because the payment was actually cleaned up).
- The **Enrollment** stayed `Active`, silently occupying a phase seat that no longer showed up anywhere in the UI.

**Fix:** `DeleteChildCommandHandler` now also fetches and soft-deletes the child's Enrollments, each Enrollment's Payments, and the child's Attendance records (added `IAttendanceRepository.GetByChildAsync` to support this).

**Re-verified live:** created a fresh child + enrollment + Absent attendance record + payment, deleted the child, then queried the database directly — `IsDeleted = 1` on all four rows (Child, Enrollment, Payment, Attendance). Dashboard numbers (`dailyAbsentCount`, `pendingPaymentsCount`, etc.) returned to the exact original baseline immediately, instead of staying permanently inflated.

### Finding 2 — High: "Add Child" failed silently if Phase or Academic Year was left unselected
**Fixed & verified live** (fixed in the previous session, re-confirmed still working).

Submitting the Add Child form with Phase/Academic Year left on their default "Select" option sent an empty string for `kgPhaseId`/`academicYearId`. The backend command expects a `Guid?`, and an empty string fails to deserialize — a 400 error, with no error message shown to the user. `child-form.component.ts` now omits these fields from the payload when unselected, matching the pattern already used in the payment form.

### Finding 3 — High: a discount entered when *creating* a payment was silently discarded
**Fixed & verified live.**

`RecordPaymentCommand` had no `Discount` field at all, even though the payment form always shows a Discount input. Reproduced: recording a new payment with AmountDue 1500, Discount 300, AmountPaid 1200 (paid in full after the discount) stored Discount as 0 and left Status as `Partial` with a 300 "remaining" balance — inflating Pending Payments by exactly that 300.

**Fix:** added `Discount` to `RecordPaymentCommand` (+ validator rule), and `Payment.RecordPayment` now nets it against `AmountDue` the same way `UpdatePayment` already did, before deciding Paid vs. Partial.

**Re-verified live:** the exact same repro (due 1500, discount 300, paid 1200) now stores `discount: 300`, `status: "Paid"`, `remaining: 0` — and no longer inflates Pending Payments.

### Finding 4 — Low: Absence Rate was diluted by inactive students
**Fixed & verified live.**

`Absent Rate = Absent Today ÷ Total Students` included children with Status = Inactive in the denominator, who obviously have no attendance taken for them — the rate would read artificially low for any nursery with withdrawn/graduated children still on record.

**Fix:** `GetDashboardSummaryQueryHandler` now divides by `activeStudents` instead of `totalStudents`.

**Re-verified live:** with 5 total students and 1 absent, temporarily marking one child Inactive (4 active) changed the rate from 20.0% to 25.0% (1÷4), confirming the new denominator; reverted immediately after.

### Finding 5 — Medium: no user-facing error feedback across most of the app
**Fixed & verified live.**

A `ToastService` (success/error/info) existed and was wired into the UI shell, but only 2 of roughly a dozen save/delete actions actually called it — everywhere else, `.subscribe({ error: ... })` just silenced the spinner. This is exactly what made Finding 2 invisible to a user, and `child-list`'s delete action had no error handler at all (an uncaught RxJS error).

**Fix:** added `toast.error(...)` to every silent create/update/delete/load failure across Children, Phases, Academic Years, Payments, and the attendance history/report screens (new `TOAST.SAVE_ERROR` / `TOAST.LOAD_ERROR` generic keys, plus specific ones for delete/toggle actions), and added proper success+error handling to `child-list`'s delete action, which previously had no error handling at all.

**Re-verified live:** created and deleted a child through the actual UI — a green "Child deleted successfully" toast now appears. Zero console errors across the whole pass.

### Finding 6 — High: Academic Year edit/activate-deactivate was completely non-functional
**Fixed & verified live.**

Wiring up Finding 5's error toast on the Academic Years screen immediately surfaced a real, pre-existing bug that had never been visible before: clicking **Edit** or **Activate/Deactivate** sent `PUT /api/v1/academicyears/{id}`, but no such endpoint existed — `AcademicYearsController` only had `GET`/`POST`, and there was no `UpdateAcademicYearCommand` anywhere in the Application layer. The request 404'd with zero visible feedback, so this had likely been silently broken since the feature was added — meaning the yearly "activate the new year, deactivate the old one" setup step could never actually be done through the UI.

**Fix:** built `UpdateAcademicYearCommand`/handler/validator and a `PUT` action on `AcademicYearsController`, mirroring the existing `UpdateKGPhaseCommand` pattern exactly (the domain entity's `Update()`/`Activate()`/`Deactivate()` methods already existed and just needed wiring up).

**A second, more systemic bug surfaced while fixing this one:** the route/body ID check (`if (id != command.Id) return BadRequest(...)`) used by *every* update endpoint — Children, Payments, KG Phases, and now Academic Years — was unconditionally rejecting every request, because none of the frontend's update calls ever send `id` in the request body (only in the URL), so `command.Id` always deserialized to `Guid.Empty` and never matched the route. **This meant Edit Child, Edit Payment, and Edit KG Phase were all silently broken (400) the same way**, not just Academic Years. Fixed by having each controller action build the command from the route ID directly (`command with { Id = id }`) instead of validating a value the frontend never sends.

**Re-verified live:** all four PUT endpoints (`children/{id}`, `payments/{id}`, `kgphases/{id}`, `academicyears/{id}`) now return `204 No Content` instead of `400`/`404`. Confirmed the academic year toggle correctly flips `isActive` without disturbing the other year, and that no test data was left behind.

## 5. What was verified correct

Using the original seed data (5 children, 3 KG1 + 2 KG2, one payment already Paid, four Unpaid) as a baseline, every dashboard figure was hand-recomputed and matched exactly:
- Total/Active Students = 5 / 5
- Absent Today = 1, Absence Rate = 1⁄5 × 100 = 20.0%
- Monthly Income = 1800.00 (the one Paid payment, correctly date-scoped to the current month)
- Pending Payments = 4 payments totaling 6300.00 (1500 × 3 + 1800, matching each unpaid child's monthly fee)
- Phase Distribution (3 / 2) and Enrollment Trends both correctly scoped to the active academic year

The Kufi Arabic font, the branding swap on printed reports, the attendance status legend, and the removed child-ID field on the child report (from earlier requests) were all re-confirmed rendering correctly during this pass.
