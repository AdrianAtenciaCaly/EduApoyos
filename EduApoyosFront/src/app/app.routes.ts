import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'login' },
    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/login/login.component').then((m) => m.LoginComponent)
    },
    {
        path: 'advisor',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Advisor'] },
        loadComponent: () =>
            import('./features/advisor/dashboard/advisor-dashboard.component').then(
                (m) => m.AdvisorDashboardComponent
            )
    },
    {
        path: 'support-requests',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Advisor'] },
        loadComponent: () =>
            import('./features/support-requests/list/support-request-list.component').then(
                (m) => m.SupportRequestListComponent
            )
    },
    {
        path: 'support-requests/new',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Advisor'] },
        loadComponent: () =>
            import('./features/support-requests/form/support-request-form.component').then(
                (m) => m.SupportRequestFormComponent
            )
    },
    {
        path: 'support-requests/:id',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/support-requests/detail/support-request-detail.component').then(
                (m) => m.SupportRequestDetailComponent
            )
    },
    {
        path: 'student',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Student'] },
        loadComponent: () =>
            import('./features/student/portal/student-portal.component').then(
                (m) => m.StudentPortalComponent
            )
    },
    { path: '**', redirectTo: 'login' }
];