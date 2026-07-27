import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';
import { Router } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatCardModule,
        MatButtonModule
    ],
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss'
})
export class LoginComponent {
    private readonly fb = inject(FormBuilder);
    private readonly auth = inject(AuthService);
    private readonly router = inject(Router);

    error = '';

    loading = false;

    hidePassword = true;

    readonly form = this.fb.nonNullable.group({
        email: [
            '',
            [
                Validators.required,
                Validators.email
            ]
        ],
        password: [
            '',
            [
                Validators.required
            ]
        ]
    });

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();

            return;
        }

        this.loading = true;
        this.error = '';

        this.auth
            .login(this.form.getRawValue())
            .subscribe({
                next: (res) => {
                    this.loading = false;

                    if (res.role === 'Advisor') {
                        this.router.navigateByUrl('/advisor');

                        return;
                    }

                    this.router.navigateByUrl('/student');
                },
                error: (err) => {
                    console.error(
                        'Login error:',
                        err
                    );

                    this.loading = false;

                    this.error =
                        'Credenciales incorrectas o el servidor no está disponible.';
                }
            });
    }

    togglePasswordVisibility(): void {
        this.hidePassword =
            !this.hidePassword;
    }
}