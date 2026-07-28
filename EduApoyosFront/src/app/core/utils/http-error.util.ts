interface ApiErrorBody {
    title?: string;
    detail?: string;
}

export function extractApiErrorMessage(error: unknown, fallback: string): string {
    if (!error || typeof error !== 'object' || !('error' in error)) {
        return fallback;
    }

    const body = (error as { error?: ApiErrorBody }).error;

    if (!body) {
        return fallback;
    }

    return body.title || body.detail || fallback;
}
