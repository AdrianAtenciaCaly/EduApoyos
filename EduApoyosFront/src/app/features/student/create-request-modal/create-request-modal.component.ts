import { Component, EventEmitter, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CreateSupportRequestRequest } from '../../../core/models/support-request.model';
import { SupportRequestService } from '../../../core/services/support-request.service';
import { extractApiErrorMessage } from '../../../core/utils/http-error.util';

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
    private readonly fb = inject(FormBuilder);
    private readonly requestsApi = inject(SupportRequestService);

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
        const body: CreateSupportRequestRequest = {
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
                this.error = extractApiErrorMessage(
                    err,
                    'No se pudo crear la solicitud de apoyo.'
                );
            }
        });
    }
}
