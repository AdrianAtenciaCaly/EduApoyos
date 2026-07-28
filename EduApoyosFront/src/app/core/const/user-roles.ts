export const USER_ROLES = {
    ADVISOR: 'Advisor',
    STUDENT: 'Student'
} as const;

export type UserRole = (typeof USER_ROLES)[keyof typeof USER_ROLES];
