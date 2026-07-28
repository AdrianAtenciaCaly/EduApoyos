import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import {
    getSupportRequestStatusLabel,
    getSupportRequestTypeLabel
} from '../../../core/const/support-request.constants';
import { StudentService } from '../../../core/services/student.service';
import { AuthService } from '../../../core/services/auth.service';
import { SupportRequestDocumentService } from '../../../core/services/support-request-document.service';
import { StudentDto } from '../../../core/models/student.model';
import { SupportRequestDto } from '../../../core/models/support-request.model';
import { CreateRequestModalComponent } from '../create-request-modal/create-request-modal.component';

@Component({
    selector: 'app-student-portal',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        MatCardModule,
        MatTableModule,
        MatButtonModule,
        CreateRequestModalComponent
    ],
    templateUrl: './student-portal.component.html',
    styleUrl: './student-portal.component.scss'
})
export class StudentPortalComponent implements OnInit {
    private readonly studentsApi = inject(StudentService);
    private readonly documentService = inject(SupportRequestDocumentService);

    readonly auth = inject(AuthService);
    readonly statusLabel = (status: string) =>
        getSupportRequestStatusLabel(status, 'feminine');
    readonly typeLabel = getSupportRequestTypeLabel;

    @ViewChild(CreateRequestModalComponent) createModal?: CreateRequestModalComponent;

    profile?: StudentDto;
    items: SupportRequestDto[] = [];
    error = '';
    message = '';
    loading = true;
    showCreateModal = false;
    columns = ['type', 'requestedAmount', 'status', 'updatedAt', 'actions'];

    ngOnInit(): void {
        this.studentsApi.getMe().subscribe({
            next: (me) => {
                this.profile = me;
                this.loading = false;
                this.loadRequests();
            },
            error: () => {
                this.loading = false;
                this.error =
                    'No se pudo cargar tu perfil de estudiante. Contacta a un asesor o verifica que tu usuario tenga rol Estudiante.';
            }
        });
    }

    openCreateModal(): void {
        if (!this.profile) {
            return;
        }

        this.message = '';
        this.error = '';
        this.showCreateModal = true;
        setTimeout(() => this.createModal?.open(this.profile!.id), 0);
    }

    onModalClosed(): void {
        this.showCreateModal = false;
    }

    onRequestCreated(): void {
        this.message = 'Solicitud de apoyo creada correctamente.';
        this.loadRequests();
    }

    loadRequests(): void {
        if (!this.profile) {
            return;
        }

        this.studentsApi.getSupportRequests(this.profile.id).subscribe({
            next: (res) => {
                this.items = res;
            },
            error: () => {
                this.error = 'No se pudieron cargar tus solicitudes de apoyo.';
            }
        });
    }

    downloadConstancyText(request: SupportRequestDto): void {
        this.documentService.downloadText(request);
    }

    downloadConstancyPdf(request: SupportRequestDto): void {
        this.documentService.downloadPdf(request);
    }
}
