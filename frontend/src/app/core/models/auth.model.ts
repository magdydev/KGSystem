export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  roles: string[];
}

export interface RegisterRequest {
  username: string;
  password: string;
  role: string;
}
