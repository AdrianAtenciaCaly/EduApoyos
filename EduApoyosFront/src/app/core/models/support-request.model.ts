export interface StatusHistoryDto {
    previousStatus: string;
    newStatus: string;
    changedAt: string;
    observation?: string | null;
}

export interface SupportRequestDto {
    id: string;
    studentId: string;
    studentName: string;
    type: string;
    requestedAmount: number;
    description: string;
    status: string;
    createdAt: string;
    updatedAt: string;
    advisorId?: string | null;
    history: StatusHistoryDto[];
}

export interface CreateSupportRequestRequest {
    studentId: string;
    type: string;
    requestedAmount: number;
    description: string;
}

export interface UpdateStatusRequest {
    newStatus: string;
    observation?: string;
}