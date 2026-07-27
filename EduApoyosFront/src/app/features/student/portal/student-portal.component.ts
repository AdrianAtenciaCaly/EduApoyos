import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { jsPDF } from 'jspdf';
import { StudentService } from '../../../core/services/student.service';
import { AuthService } from '../../../core/services/auth.service';
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
    private studentsApi = inject(StudentService);
    auth = inject(AuthService);

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
        // Espera un tick para que el ViewChild exista
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

    statusLabel(status: string): string {
        const map: Record<string, string> = {
            Pending: 'Pendiente',
            UnderReview: 'En revisión',
            Approved: 'Aprobada',
            Rejected: 'Rechazada'
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

    downloadConstancyText(r: SupportRequestDto): void {
        const text = this.buildConstancyText(r);
        const blob = new Blob([text], { type: 'text/plain;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `constancia-${r.id}.txt`;
        a.click();
        URL.revokeObjectURL(url);
    }

    downloadConstancyPdf(r: SupportRequestDto): void {
        const doc = new jsPDF();
        const lines = this.buildConstancyText(r).split('\n');
        doc.setFontSize(14);
        doc.text('EduApoyos — Constancia de solicitud de apoyo', 14, 20);
        doc.setFontSize(11);
        let y = 32;
        for (const line of lines) {
            if (y > 280) {
                doc.addPage();
                y = 20;
            }
            doc.text(line, 14, y);
            y += 8;
        }
        doc.save(`constancia-${r.id}.pdf`);
    }

    private buildConstancyText(r: SupportRequestDto): string {
        return [
            'EduApoyos — Constancia de solicitud de apoyo',
            '==========================================',
            `Estudiante: ${r.studentName}`,
            `Id de solicitud: ${r.id}`,
            `Tipo: ${this.typeLabel(r.type)}`,
            `Monto: ${r.requestedAmount}`,
            `Estado: ${this.statusLabel(r.status)}`,
            `Creada: ${r.createdAt}`,
            `Actualizada: ${r.updatedAt}`,
            `Descripción: ${r.description}`,
            '',
            'Este documento certifica el estado actual de la solicitud',
            'en el sistema EduApoyos.',
            `Generado: ${new Date().toLocaleString('es-CO')}`
        ].join('\n');
    }
}