import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { USER_ROLES } from '../const/user-roles';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'eduapoyos_token';
const USER_KEY = 'eduapoyos_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);
    private router = inject(Router);
    private currentUser$ = new BehaviorSubject<AuthResponse | null>(this.readUser());

    readonly user$ = this.currentUser$.asObservable();

    login(body: LoginRequest) {
        return this.http
            .post<AuthResponse>(`${environment.apiUrl}/auth/login`, body)
            .pipe(tap((res) => this.persist(res)));
    }

    register(body: RegisterRequest) {
        return this.http
            .post<AuthResponse>(`${environment.apiUrl}/auth/register`, body)
            .pipe(tap((res) => this.persist(res)));
    }

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        this.currentUser$.next(null);
        this.router.navigate(['/login']);
    }

    getToken(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }

    getRole(): string | null {
        return this.currentUser$.value?.role ?? null;
    }

    getUserId(): string | null {
        return this.currentUser$.value?.userId ?? null;
    }

    getFullName(): string | null {
        return this.currentUser$.value?.fullName ?? null;
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }

    isAdvisor(): boolean {
        return this.getRole() === USER_ROLES.ADVISOR;
    }

    isStudent(): boolean {
        return this.getRole() === USER_ROLES.STUDENT;
    }

    private persist(res: AuthResponse): void {
        localStorage.setItem(TOKEN_KEY, res.token);
        localStorage.setItem(USER_KEY, JSON.stringify(res));
        this.currentUser$.next(res);
    }

    private readUser(): AuthResponse | null {
        const raw = localStorage.getItem(USER_KEY);
        if (!raw) return null;
        try {
            return JSON.parse(raw) as AuthResponse;
        } catch {
            return null;
        }
    }
}