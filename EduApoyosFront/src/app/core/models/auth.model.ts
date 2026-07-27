export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    fullName: string;
    email: string;
    password: string;
    role: string;
    documentNumber?: string;
    documentType?: string;
    academicProgram?: string;
    semester?: number;
}

export interface AuthResponse {
    token: string;
    expiration: string;
    userId: string;
    fullName: string;
    email: string;
    role: string;
}