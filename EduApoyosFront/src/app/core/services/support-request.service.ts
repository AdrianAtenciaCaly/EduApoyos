import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/student.model';
import {
    CreateSupportRequestRequest,
    SupportRequestDto,
    UpdateStatusRequest
} from '../models/support-request.model';

@Injectable({ providedIn: 'root' })
export class SupportRequestService {
    private http = inject(HttpClient);
    private base = `${environment.apiUrl}/support-requests`;

    getAll(page = 1, pageSize = 10, status?: string, type?: string) {
        let params = new HttpParams().set('page', page).set('pageSize', pageSize);
        if (status) params = params.set('status', status);
        if (type) params = params.set('type', type);
        return this.http.get<PagedResult<SupportRequestDto>>(this.base, { params });
    }

    getById(id: string) {
        return this.http.get<SupportRequestDto>(`${this.base}/${id}`);
    }

    create(body: CreateSupportRequestRequest) {
        return this.http.post<SupportRequestDto>(this.base, body);
    }

    updateStatus(id: string, body: UpdateStatusRequest) {
        return this.http.patch<SupportRequestDto>(`${this.base}/${id}/status`, body);
    }
}