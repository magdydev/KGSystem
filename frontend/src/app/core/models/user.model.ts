export interface SystemUser {
  id: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface CreateUserRequest {
  username: string;
  password: string;
  roles: string[];
}

export interface UpdateUserRolesRequest {
  roles: string[];
}
