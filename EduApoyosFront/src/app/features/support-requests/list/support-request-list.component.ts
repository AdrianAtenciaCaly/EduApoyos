import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormBuilder,
    ReactiveFormsModule
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import {
    MatPaginatorModule,
    PageEvent
} from '@angular/material/paginator';

import {
    getSupportRequestStatusLabel,
    getSupportRequestTypeLabel
} from '../../../core/const/support-request.constants';
import { SupportRequestService } from '../../../core/services/support-request.service';
import { SupportRequestDto } from '../../../core/models/support-request.model';

@Component({
    selector: 'app-support-request-list',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink,
        MatCardModule,
        MatTableModule,
        MatFormFieldModule,
        MatSelectModule,
        MatButtonModule,
        MatPaginatorModule
    ],
    templateUrl: './support-request-list.component.html',
    styleUrl: './support-request-list.component.scss'
})
export class SupportRequestListComponent implements OnInit {
    private readonly api = inject(SupportRequestService);
    private readonly fb = inject(FormBuilder);

    readonly getStatusLabel = getSupportRequestStatusLabel;
    readonly getTypeLabel = getSupportRequestTypeLabel;

    columns = [
        'studentName',
        'type',
        'requestedAmount',
        'status',
        'createdAt',
        'actions'
    ];

    items: SupportRequestDto[] = [];
    total = 0;
    page = 1;
    pageSize = 10;
    error = '';

    filters = this.fb.nonNullable.group({
        status: [''],
        type: ['']
    });

    ngOnInit(): void {
        this.load();
    }

    get hasActiveFilters(): boolean {
        const { status, type } = this.filters.getRawValue();
        return Boolean(status || type);
    }

    load(): void {
        const { status, type } = this.filters.getRawValue();

        this.api
            .getAll(
                this.page,
                this.pageSize,
                status || undefined,
                type || undefined
            )
            .subscribe({
                next: (res) => {
                    this.items = res.items;
                    this.total = res.totalCount;
                    this.error = '';
                },
                error: () => {
                    this.error = 'No fue posible cargar las solicitudes de apoyo.';
                }
            });
    }

    applyFilters(): void {
        this.page = 1;
        this.load();
    }

    clearFilters(): void {
        this.filters.reset({
            status: '',
            type: ''
        });
        this.page = 1;
        this.load();
    }

    removeFilter(filter: 'status' | 'type'): void {
        this.filters.controls[filter].setValue('');
        this.page = 1;
        this.load();
    }

    onPage(event: PageEvent): void {
        this.page = event.pageIndex + 1;
        this.pageSize = event.pageSize;
        this.load();
    }
}
