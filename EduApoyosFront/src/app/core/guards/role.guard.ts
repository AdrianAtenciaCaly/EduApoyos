import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const roles = (route.data['roles'] as string[]) ?? [];

    if (!auth.isLoggedIn()) return router.createUrlTree(['/login']);
    const role = auth.getRole();
    if (role && roles.includes(role)) return true;
    return router.createUrlTree([role === 'Student' ? '/student' : '/advisor']);
};