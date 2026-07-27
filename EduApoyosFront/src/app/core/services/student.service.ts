import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CreateStudentRequest, PagedResult, StudentDto } from '../models/student.model';
import { SupportRequestDto } from '../models/support-request.model';

@Injectable({ providedIn: 'root' })
export class StudentService {
    private http = inject(HttpClient);
    private base = `${environment.apiUrl}/students`;

    getAll(page = 1, pageSize = 10) {
        const params = new HttpParams().set('page', page).set('pageSize', pageSize);
        return this.http.get<PagedResult<StudentDto>>(this.base, { params });
    }

    create(body: CreateStudentRequest) {
        return this.http.post<StudentDto>(this.base, body);
    }

    getSupportRequests(studentId: string) {
        return this.http.get<SupportRequestDto[]>(`${this.base}/${studentId}/support-requests`);
    }

    getMe() {
        return this.http.get<StudentDto>(`${this.base}/me`);
    }
}