export interface StudentDto {
    id: string;
    userId: string;
    fullName: string;
    email: string;
    documentNumber: string;
    documentType: string;
    academicProgram: string;
    semester: number;
}

export interface CreateStudentRequest {
    fullName: string;
    email: string;
    password: string;
    documentNumber: string;
    documentType: string;
    academicProgram: string;
    semester: number;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}