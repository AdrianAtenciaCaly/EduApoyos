import { Component, inject } from '@angular/core';

import {
    FormBuilder,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';

import {
    MatDialogModule,
    MatDialogRef
} from '@angular/material/dialog';

import {
    MatFormFieldModule
} from '@angular/material/form-field';

import {
    MatInputModule
} from '@angular/material/input';

import {
    MatSelectModule
} from '@angular/material/select';

import {
    MatButtonModule
} from '@angular/material/button';

import {
    StudentService
} from '../../../core/services/student.service';
import { DOCUMENT_TYPES } from '../../../core/models/document-types';
import { ACADEMIC_PROGRAMS } from '../../../core/const/academic-programs';

@Component({
    selector: 'app-student-create-dialog',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        MatDialogModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule
    ],
    templateUrl: './student-create-dialog.component.html',
    styleUrl: './student-create-dialog.component.scss'
})
export class StudentCreateDialogComponent {

    private readonly fb = inject(FormBuilder);

    private readonly studentsApi = inject(
        StudentService
    );

    private readonly dialogRef = inject(
        MatDialogRef<StudentCreateDialogComponent>
    );

    error = '';
    documentTypes = DOCUMENT_TYPES;
    academicPrograms = ACADEMIC_PROGRAMS;
    hidePassword = true;
    form = this.fb.nonNullable.group({

        fullName: [
            '',
            Validators.required
        ],

        email: [
            '',
            [
                Validators.required,
                Validators.email
            ]
        ],

        password: [
            'Student123*',
            Validators.required
        ],

        documentNumber: [
            '',
            Validators.required
        ],

        documentType: [
            'CC',
            Validators.required
        ],

        academicProgram: [
            '',
            Validators.required
        ],

        semester: [
            1,
            [
                Validators.required,
                Validators.min(1)
            ]
        ]

    });

    togglePassword(): void {

        this.hidePassword =
            !this.hidePassword;

    }

    save(): void {

        if (this.form.invalid) {

            this.form.markAllAsTouched();

            return;
        }

        this.error = '';

        this.studentsApi
            .create(
                this.form.getRawValue()
            )
            .subscribe({

                next: () => {

                    this.dialogRef.close(true);

                },

                error: (err) => {

                    this.error =
                        err?.error?.title ||
                        err?.error?.detail ||
                        'No fue posible registrar el estudiante.';

                }

            });

    }

    close(): void {

        this.dialogRef.close(false);

    }

}