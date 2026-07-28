import { Injectable } from '@angular/core';
import { jsPDF } from 'jspdf';
import {
    getSupportRequestStatusLabel,
    getSupportRequestTypeLabel
} from '../const/support-request.constants';
import { SupportRequestDto } from '../models/support-request.model';

@Injectable({ providedIn: 'root' })
export class SupportRequestDocumentService {
    downloadText(request: SupportRequestDto): void {
        const text = this.buildConstancyText(request);
        const blob = new Blob([text], { type: 'text/plain;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');

        anchor.href = url;
        anchor.download = `constancia-${request.id}.txt`;
        anchor.click();
        URL.revokeObjectURL(url);
    }

    downloadPdf(request: SupportRequestDto): void {
        const doc = new jsPDF();
        const lines = this.buildConstancyText(request).split('\n');

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

        doc.save(`constancia-${request.id}.pdf`);
    }

    private buildConstancyText(request: SupportRequestDto): string {
        return [
            'EduApoyos — Constancia de solicitud de apoyo',
            '==========================================',
            `Estudiante: ${request.studentName}`,
            `Id de solicitud: ${request.id}`,
            `Tipo: ${getSupportRequestTypeLabel(request.type)}`,
            `Monto: ${request.requestedAmount}`,
            `Estado: ${getSupportRequestStatusLabel(request.status, 'feminine')}`,
            `Creada: ${request.createdAt}`,
            `Actualizada: ${request.updatedAt}`,
            `Descripción: ${request.description}`,
            '',
            'Este documento certifica el estado actual de la solicitud',
            'en el sistema EduApoyos.',
            `Generado: ${new Date().toLocaleString('es-CO')}`
        ].join('\n');
    }
}
