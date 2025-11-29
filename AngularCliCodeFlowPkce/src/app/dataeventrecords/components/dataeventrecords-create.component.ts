import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { OidcSecurityService, ConfigAuthenticatedResult, AuthenticatedResult } from 'angular-auth-oidc-client';

import { DataEventRecordsService } from '../dataeventrecords.service';
import { DataEventRecord } from '../models/DataEventRecord';

@Component({
    selector: 'app-dataeventrecords-create',
    templateUrl: 'dataeventrecords-create.component.html',
    standalone: false
})

export class DataEventRecordsCreateComponent implements OnInit {

    message: string;
    DataEventRecord: DataEventRecord = {
        id: 0, name: '', description: '', timestamp: ''
    };

    isAuthenticated$: Observable<AuthenticatedResult>;

    constructor(private _dataEventRecordsService: DataEventRecordsService,
        public oidcSecurityService: OidcSecurityService,
        private _router: Router
    ) {
        this.message = 'DataEventRecords Create';
        this.isAuthenticated$ = this.oidcSecurityService.isAuthenticated$;
    }

    ngOnInit() {
        this.isAuthenticated$.subscribe((isAuthenticated: AuthenticatedResult) => {
            console.log('isAuthorized: ' + isAuthenticated.isAuthenticated);
        });

        this.DataEventRecord = { id: 0, name: '', description: '', timestamp: '' };
    }

     Create() {
        // router navigate to DataEventRecordsList
        this._dataEventRecordsService
            .Add(this.DataEventRecord)
            .subscribe((data: any) => this.DataEventRecord = data,
            () => this._router.navigate(['/dataeventrecords']));
    }
}
