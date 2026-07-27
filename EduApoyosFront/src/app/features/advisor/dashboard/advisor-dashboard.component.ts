import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import {
    MatPaginatorModule,
    PageEvent
} from '@angular/material/paginator';
import {
    MatDialog,
    MatDialogModule
} from '@angular/material/dialog';

import { StudentService } from '../../../core/services/student.service';
import { StudentDto } from '../../../core/models/student.model';
import { StudentCreateDialogComponent } from '../../student/student-create-dialog/student-create-dialog.component';


@Component({
    selector: 'app-advisor-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        MatCardModule,
        MatTableModule,
        MatButtonModule,
        MatPaginatorModule,
        MatDialogModule
    ],
    templateUrl: './advisor-dashboard.component.html',
    styleUrl: './advisor-dashboard.component.scss'
})
export class AdvisorDashboardComponent implements OnInit {

    private readonly studentsApi = inject(StudentService);
    private readonly dialog = inject(MatDialog);

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

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.studentsApi
            .getAll(this.page, this.pageSize)
            .subscribe({
                next: (res) => {
                    this.items = res.items;
                    this.total = res.totalCount;
                    this.error = '';
                },
                error: () => {
                    this.error =
                        'No fue posible cargar los estudiantes.';
                }
            });
    }

    openCreateModal(): void {
        this.message = '';
        this.error = '';

        const dialogRef = this.dialog.open(
            StudentCreateDialogComponent,
            {
                width: '620px',
                maxWidth: '95vw',
                maxHeight: '90vh',
                autoFocus: false,
                panelClass: 'student-dialog'
            }
        );

        dialogRef.afterClosed().subscribe({
            next: (created: boolean) => {
                if (created) {
                    this.message =
                        'Estudiante registrado correctamente.';

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

        return fullName
            .trim()
            .charAt(0)
            .toUpperCase();
    }
}