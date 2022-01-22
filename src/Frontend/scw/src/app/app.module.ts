import { environment } from './../environments/environment';
import { RemoveCollaborationsDialogComponent } from './Dialogs/remove-collaborations-dialog/remove-collaborations-dialog.component';
import { AddCollaboratorDialogComponent } from './Dialogs/add-collaborator-dialog/add-collaborator-dialog.component';

import { AllCollaborationsComponent } from './all-collaborations/all-collaborations.component';

//Components
import { AppComponent } from './app.component';
import { LogInSignUpDialogComponent } from './Dialogs/log-in-sign-up-dialog/log-in-sign-up-dialog.component';
import { AllTablesComponent } from './all-tables/all-tables.component';
//Materials
import {MatDialogModule} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatIconModule} from '@angular/material/icon';
import {MatMenuModule} from '@angular/material/menu';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatSelectModule} from '@angular/material/select';
import {MatSnackBarModule} from '@angular/material/snack-bar';


//3rd party stuff
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {HttpClientModule} from '@angular/common/http'
import {CommonModule} from '@angular/common';
import { FormsModule } from '@angular/forms';
import {MatTabsModule} from '@angular/material/tabs';
import { SheetComponent } from './sheet/sheet.component';
import { DatasetComponent } from './dataset/dataset.component';
import { DataSetDialogComponent } from './Dialogs/data-set-dialog/data-set-dialog.component';
import { MatTableModule } from '@angular/material/table';

//primeNG stuff
//import {AccordionModule} from 'primeng/accordion';     //accordion and accordion tab               //api
import {MatListModule} from '@angular/material/list';
import {ToastModule} from 'primeng/toast';
import {CalendarModule} from 'primeng/calendar';
import {SliderModule} from 'primeng/slider';
import {MultiSelectModule} from 'primeng/multiselect';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DialogModule} from 'primeng/dialog';
import {ButtonModule} from 'primeng/button';
import {DropdownModule} from 'primeng/dropdown';
import {ProgressBarModule} from 'primeng/progressbar';
import {InputTextModule} from 'primeng/inputtext';
import {FileUploadModule} from 'primeng/fileupload';
import {TableModule} from "primeng/table";
import { MenubarComponent } from './menubar/menubar.component';
import {MenubarModule} from "primeng/menubar";
import { InMemoryCache } from '@apollo/client/cache/inmemory/inMemoryCache';
import { SpreadjsComponent } from './spreadjs/spreadjs.component';

import { DesignerModule } from '@grapecity/spread-sheets-designer-angular'
import { RouterModule } from '@angular/router';
import { APOLLO_OPTIONS } from 'apollo-angular';
import {HttpLink} from 'apollo-angular/http';
import { AddColumnDialogComponent } from './Dialogs/add-column-dialog/add-column-dialog.component';
import { DeleteColumnDialogComponent } from './Dialogs/delete-column-dialog/delete-column-dialog.component';
import { AdminUserListComponent } from './admin-user-list/admin-user-list.component';
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {RadioButtonModule} from "primeng/radiobutton";
import {InputNumberModule} from "primeng/inputnumber";
import { AdminStatisticsPageComponent } from './admin-statistics-page/admin-statistics-page.component';
import {ChartModule} from "primeng/chart";
import { CreateRowDialogComponent } from './Dialogs/create-row-dialog/create-row-dialog.component';
import { ShowTablesOfUserDialogComponent } from './Dialogs/show-tables-of-user-dialog/show-tables-of-user-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LogInSignUpDialogComponent,
    AllTablesComponent,
    SheetComponent,
    DatasetComponent,
    DataSetDialogComponent,
    MenubarComponent,
    AddColumnDialogComponent,
    DeleteColumnDialogComponent,
    SpreadjsComponent,
    AllCollaborationsComponent,
    AddCollaboratorDialogComponent,
    RemoveCollaborationsDialogComponent,
    AdminUserListComponent,
    AdminStatisticsPageComponent,
    CreateRowDialogComponent,
    ShowTablesOfUserDialogComponent,
    AdminUserListComponent,
    AdminStatisticsPageComponent,
    CreateRowDialogComponent,
    ShowTablesOfUserDialogComponent
  ],
  imports: [
    DesignerModule,
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    HttpClientModule,
    MatDialogModule,
    MatFormFieldModule,
    CommonModule,
    MatInputModule,
    FormsModule,
    MatToolbarModule,
    MatSidenavModule,
    MatIconModule,
    MatTabsModule,
    MatMenuModule,
    MatTableModule,
    MatCheckboxModule,
    MatSelectModule,
    RouterModule,
    BrowserModule,
    BrowserAnimationsModule,
    CalendarModule,
    SliderModule,
    DialogModule,
    MultiSelectModule,
    ContextMenuModule,
    DropdownModule,
    ButtonModule,
    ToastModule,
    InputTextModule,
    ProgressBarModule,
    HttpClientModule,
    FormsModule,
    FileUploadModule,
    TableModule,
    MenubarModule,
    BrowserModule,
    BrowserAnimationsModule,
    MenubarModule,
    InputTextModule,
    ButtonModule,
    MatListModule,
    MatSnackBarModule,
    ConfirmDialogModule,
    RadioButtonModule,
    InputNumberModule,
    ChartModule
  ],
  providers: [{
    provide: APOLLO_OPTIONS,
    useFactory: (httpLink: HttpLink) => {
      return {
        cache: new InMemoryCache(),
        link: httpLink.create({
          uri: environment.graphqlUri,
        }),
      };
    },
    deps: [HttpLink],
  },],
  bootstrap: [AppComponent]
})
export class AppModule { }
