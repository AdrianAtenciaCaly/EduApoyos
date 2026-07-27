import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { SupportRequestService } from '../../../core/services/support-request.service';
import { AuthService } from '../../../core/services/auth.service';
import { SupportRequestDto } from '../../../core/models/support-request.model';

@Component({
    selector: 'app-support-request-detail',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink,
        MatCardModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatButtonModule
    ],
    templateUrl: './support-request-detail.component.html',
    styleUrl: './support-request-detail.component.scss'
})
export class SupportRequestDetailComponent implements OnInit {
    private readonly route = inject(ActivatedRoute);
    private readonly api = inject(SupportRequestService);
    private readonly fb = inject(FormBuilder);

    auth = inject(AuthService);

    item?: SupportRequestDto;
    error = '';
    message = '';
    saving = false;
    private currentId = '';

    statusForm = this.fb.nonNullable.group({
        newStatus: ['UnderReview', Validators.required],
        observation: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(500)]]
    });

    /** Aprobado o Rechazado: no se puede volver a cambiar */
    get isFinalStatus(): boolean {
        const status = this.item?.status;
        return status === 'Approved' || status === 'Rejected';
    }

    get canChangeStatus(): boolean {
        return this.auth.getRole() === 'Advisor' && !!this.item && !this.isFinalStatus;
    }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (!id) {
            this.error = 'Identificador de solicitud inválido.';
            return;
        }
        this.currentId = id;
        this.load(id);
    }

    load(id: string): void {
        this.error = '';
        this.api.getById(id).subscribe({
            next: (res) => {
                this.item = {
                    ...res,
                    history: [...(res.history ?? [])]
                };
                this.statusForm.patchValue({
                    newStatus: res.status,
                    observation: ''
                });
            },
            error: () => {
                this.error = 'Solicitud no encontrada o acceso denegado.';
            }
        });
    }

    refresh(): void {
        if (this.currentId) {
            this.load(this.currentId);
        }
    }

    changeStatus(): void {
        if (!this.item || this.isFinalStatus) {
            this.error = 'Esta solicitud ya está cerrada y no puede actualizarse.';
            return;
        }

        if (this.statusForm.invalid) {
            this.statusForm.markAllAsTouched();
            this.error = 'Debes indicar el nuevo estado y una observación (mínimo 3 caracteres).';
            return;
        }

        const { newStatus, observation } = this.statusForm.getRawValue();
        const comment = observation.trim();

        if (!comment) {
            this.statusForm.controls.observation.setErrors({ required: true });
            this.error = 'La observación es obligatoria al cambiar el estado.';
            return;
        }

        this.error = '';
        this.message = '';
        this.saving = true;

        this.api
            .updateStatus(this.item.id, {
                newStatus,
                observation: comment
            })
            .subscribe({
                next: () => {
                    this.saving = false;
                    this.message = 'Estado actualizado correctamente.';
                    this.refresh();
                },
                error: (err) => {
                    this.saving = false;
                    this.error =
                        err?.error?.title ||
                        err?.error?.detail ||
                        'No fue posible actualizar el estado.';
                }
            });
    }

    statusLabel(status: string): string {
        const map: Record<string, string> = {
            Pending: 'Pendiente',
            UnderReview: 'En revisión',
            Approved: 'Aprobado',
            Rejected: 'Rechazado'
        };
        return map[status] ?? status;
    }

    typeLabel(type: string): string {
        const map: Record<string, string> = {
            Scholarship: 'Beca',
            Credit: 'Crédito',
            Subsidy: 'Subsidio'
        };
        return map[type] ?? type;
    }
}