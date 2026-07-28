export const SUPPORT_REQUEST_STATUS = {
    PENDING: 'Pending',
    UNDER_REVIEW: 'UnderReview',
    APPROVED: 'Approved',
    REJECTED: 'Rejected'
} as const;

export const SUPPORT_REQUEST_TYPE = {
    SCHOLARSHIP: 'Scholarship',
    CREDIT: 'Credit',
    SUBSIDY: 'Subsidy'
} as const;

export type SupportRequestStatus =
    (typeof SUPPORT_REQUEST_STATUS)[keyof typeof SUPPORT_REQUEST_STATUS];

export type SupportRequestType =
    (typeof SUPPORT_REQUEST_TYPE)[keyof typeof SUPPORT_REQUEST_TYPE];

type LabelGender = 'masculine' | 'feminine';

const STATUS_LABELS: Record<SupportRequestStatus, Record<LabelGender, string>> = {
    [SUPPORT_REQUEST_STATUS.PENDING]: {
        masculine: 'Pendiente',
        feminine: 'Pendiente'
    },
    [SUPPORT_REQUEST_STATUS.UNDER_REVIEW]: {
        masculine: 'En revisión',
        feminine: 'En revisión'
    },
    [SUPPORT_REQUEST_STATUS.APPROVED]: {
        masculine: 'Aprobado',
        feminine: 'Aprobada'
    },
    [SUPPORT_REQUEST_STATUS.REJECTED]: {
        masculine: 'Rechazado',
        feminine: 'Rechazada'
    }
};

const TYPE_LABELS: Record<SupportRequestType, string> = {
    [SUPPORT_REQUEST_TYPE.SCHOLARSHIP]: 'Beca',
    [SUPPORT_REQUEST_TYPE.CREDIT]: 'Crédito',
    [SUPPORT_REQUEST_TYPE.SUBSIDY]: 'Subsidio'
};

export function getSupportRequestStatusLabel(
    status: string,
    gender: LabelGender = 'masculine'
): string {
    const labels = STATUS_LABELS[status as SupportRequestStatus];
    return labels?.[gender] ?? status;
}

export function getSupportRequestTypeLabel(type: string): string {
    return TYPE_LABELS[type as SupportRequestType] ?? type;
}

export function isFinalSupportRequestStatus(status: string): boolean {
    return (
        status === SUPPORT_REQUEST_STATUS.APPROVED ||
        status === SUPPORT_REQUEST_STATUS.REJECTED
    );
}
