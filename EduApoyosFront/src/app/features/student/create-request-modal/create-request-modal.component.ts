import { Component, EventEmitter, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SupportRequestService } from '../../../core/services/support-request.service';

export interface CreateRequestPayload {
    studentId: string;
    type: string;
    requestedAmount: number;
    description: string;
}

@Component({
    selector: 'app-create-request-modal',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule
    ],
    templateUrl: './create-request-modal.component.html',
    styleUrl: './create-request-modal.component.scss'
})
export class CreateRequestModalComponent {
    private fb = inject(FormBuilder);
    private requestsApi = inject(SupportRequestService);

    /** Student id del perfil logueado (lo pasa el portal) */
    studentId = '';

    @Output() closed = new EventEmitter<void>();
    @Output() created = new EventEmitter<void>();

    error = '';
    saving = false;

    form = this.fb.nonNullable.group({
        type: ['Scholarship', Validators.required],
        requestedAmount: [null as number | null, [Validators.required, Validators.min(1)]],
        description: ['', [Validators.required, Validators.maxLength(500)]]
    });

    /** Llamar desde el padre al abrir, con el id del estudiante */
    open(studentId: string): void {
        this.studentId = studentId;
        this.error = '';
        this.form.reset({
            type: 'Scholarship',
            requestedAmount: null,
            description: ''
        });
    }

    close(): void {
        this.closed.emit();
    }

    onBackdropClick(): void {
        this.close();
    }

    submit(): void {
        if (!this.studentId || this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.saving = true;
        this.error = '';

        const raw = this.form.getRawValue();
        const body: CreateRequestPayload = {
            studentId: this.studentId,
            type: raw.type,
            requestedAmount: Number(raw.requestedAmount),
            description: raw.description
        };

        this.requestsApi.create(body).subscribe({
            next: () => {
                this.saving = false;
                this.created.emit();
                this.close();
            },
            error: (err) => {
                this.saving = false;
                this.error =
                    err?.error?.title ||
                    err?.error?.detail ||
                    'No se pudo crear la solicitud de apoyo.';
            }
        });
    }
}