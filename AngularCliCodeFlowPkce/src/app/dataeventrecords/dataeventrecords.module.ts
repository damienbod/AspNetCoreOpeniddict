import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { DataEventRecordsService } from './dataeventrecords.service';
import { DataEventRecordsListComponent } from './components/dataeventrecords-list.component';
import { DataEventRecordsCreateComponent } from './components/dataeventrecords-create.component';
import { DataEventRecordsEditComponent } from './components/dataeventrecords-edit.component';
import { DataEventRecordsRoutes } from './dataeventrecords.routes';

@NgModule({ declarations: [
        DataEventRecordsListComponent,
        DataEventRecordsCreateComponent,
        DataEventRecordsEditComponent
    ],
    exports: [
        DataEventRecordsListComponent,
        DataEventRecordsCreateComponent,
        DataEventRecordsEditComponent
    ], imports: [CommonModule,
        FormsModule,
        DataEventRecordsRoutes], providers: [
        DataEventRecordsService,
        provideHttpClient(withInterceptorsFromDi())
    ] })

export class DataEventRecordsModule { }
