import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';
import {
    Router,
    RouterLink
} from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

import { StudentService } from '../../../core/services/student.service';
import { SupportRequestService } from '../../../core/services/support-request.service';
import { StudentDto } from '../../../core/models/student.model';

@Component({
    selector: 'app-support-request-form',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule
    ],
    templateUrl: './support-request-form.component.html',
    styleUrl: './support-request-form.component.scss'
})
export class SupportRequestFormComponent implements OnInit {
    private readonly fb = inject(FormBuilder);
    private readonly studentsApi = inject(StudentService);
    private readonly requestsApi = inject(SupportRequestService);
    private readonly router = inject(Router);

    students: StudentDto[] = [];
    error = '';

    form = this.fb.nonNullable.group({
        studentId: ['', Validators.required],
        type: ['Scholarship', Validators.required],
        requestedAmount: [
            0,
            [
                Validators.required,
                Validators.min(1)
            ]
        ],
        description: [
            '',
            [
                Validators.required,
                Validators.maxLength(500)
            ]
        ]
    });

    ngOnInit(): void {
        this.loadStudents();
    }

    loadStudents(): void {
        this.studentsApi
            .getAll(1, 100)
            .subscribe({
                next: (res) => {
                    this.students = res.items;
                },
                error: () => {
                    this.error = 'No fue posible cargar los estudiantes.';
                }
            });
    }

    submit(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.error = '';

        this.requestsApi
            .create(this.form.getRawValue())
            .subscribe({
                next: (res) => {
                    this.router.navigate([
                        '/support-requests',
                        res.id
                    ]);
                },
                error: (err) => {
                    this.error =
                        err?.error?.title ||
                        err?.error?.detail ||
                        'No fue posible crear la solicitud.';
                }
            });
    }
}