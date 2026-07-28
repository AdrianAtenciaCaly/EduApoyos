import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import {
    MatPaginatorModule,
    PageEvent
} from '@angular/material/paginator';
import {
    MatDialog,
    MatDialogModule
} from '@angular/material/dialog';

import { ACADEMIC_PROGRAMS } from '../../../core/const/academic-programs';
import { DOCUMENT_TYPES } from '../../../core/models/document-types';
import { StudentService } from '../../../core/services/student.service';
import { StudentDto } from '../../../core/models/student.model';
import { StudentCreateDialogComponent } from '../../student/student-create-dialog/student-create-dialog.component';

@Component({
    selector: 'app-advisor-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatCardModule,
        MatTableModule,
        MatButtonModule,
        MatFormFieldModule,
        MatSelectModule,
        MatPaginatorModule,
        MatDialogModule
    ],
    templateUrl: './advisor-dashboard.component.html',
    styleUrl: './advisor-dashboard.component.scss'
})
export class AdvisorDashboardComponent implements OnInit {
    private readonly studentsApi = inject(StudentService);
    private readonly dialog = inject(MatDialog);
    private readonly fb = inject(FormBuilder);

    readonly academicPrograms = ACADEMIC_PROGRAMS;
    readonly documentTypes = DOCUMENT_TYPES;
    readonly semesterOptions = Array.from({ length: 12 }, (_, index) => index + 1);

    displayedColumns = [
        'fullName',
        'email',
        'documentNumber',
        'academicProgram',
        'semester'
    ];

    items: StudentDto[] = [];
    total = 0;
    page = 1;
    pageSize = 10;

    message = '';
    error = '';

    filters = this.fb.nonNullable.group({
        academicProgram: [''],
        documentType: [''],
        semester: ['']
    });

    ngOnInit(): void {
        this.load();
    }

    get hasActiveFilters(): boolean {
        const { academicProgram, documentType, semester } = this.filters.getRawValue();
        return Boolean(academicProgram || documentType || semester);
    }

    load(): void {
        const fetchPageSize = this.hasActiveFilters ? 100 : this.pageSize;
        const fetchPage = this.hasActiveFilters ? 1 : this.page;

        this.studentsApi.getAll(fetchPage, fetchPageSize).subscribe({
            next: (res) => {
                const filtered = this.applyClientFilters(res.items);

                if (this.hasActiveFilters) {
                    this.total = filtered.length;
                    const start = (this.page - 1) * this.pageSize;
                    this.items = filtered.slice(start, start + this.pageSize);
                } else {
                    this.items = filtered;
                    this.total = res.totalCount;
                }

                this.error = '';
            },
            error: () => {
                this.error = 'No fue posible cargar los estudiantes.';
            }
        });
    }

    applyFilters(): void {
        this.page = 1;
        this.load();
    }

    clearFilters(): void {
        this.filters.reset({
            academicProgram: '',
            documentType: '',
            semester: ''
        });
        this.page = 1;
        this.load();
    }

    removeFilter(filter: 'academicProgram' | 'documentType' | 'semester'): void {
        this.filters.controls[filter].setValue('');
        this.page = 1;
        this.load();
    }

    getDocumentTypeLabel(value: string): string {
        return this.documentTypes.find((type) => type.value === value)?.label ?? value;
    }

    openCreateModal(): void {
        this.message = '';
        this.error = '';

        const dialogRef = this.dialog.open(StudentCreateDialogComponent, {
            width: '620px',
            maxWidth: '95vw',
            maxHeight: '90vh',
            autoFocus: false,
            panelClass: 'student-dialog'
        });

        dialogRef.afterClosed().subscribe({
            next: (created: boolean) => {
                if (created) {
                    this.message = 'Estudiante registrado correctamente.';
                    this.load();
                }
            }
        });
    }

    onPage(event: PageEvent): void {
        this.page = event.pageIndex + 1;
        this.pageSize = event.pageSize;
        this.load();
    }

    getInitial(fullName: string): string {
        if (!fullName) {
            return '?';
        }

        return fullName.trim().charAt(0).toUpperCase();
    }

    private applyClientFilters(students: StudentDto[]): StudentDto[] {
        const { academicProgram, documentType, semester } = this.filters.getRawValue();

        return students.filter((student) => {
            if (academicProgram && student.academicProgram !== academicProgram) {
                return false;
            }

            if (documentType && student.documentType !== documentType) {
                return false;
            }

            if (semester && student.semester !== Number(semester)) {
                return false;
            }

            return true;
        });
    }
}
