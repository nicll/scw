//Components
import { AppComponent } from './app.component';
import { HandsontestComponent } from './handsontest/handsontest.component';
import { LogInSignUpDialogComponent } from './log-in-sign-up-dialog/log-in-sign-up-dialog.component';
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
//3rd party stuff
import { HotTableModule } from '@handsontable/angular';

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
import { DataSetDialogComponent } from './data-set-dialog/data-set-dialog.component';
import {MatTableModule} from '@angular/material/table';
@NgModule({
  declarations: [
    AppComponent,
    HandsontestComponent,
    LogInSignUpDialogComponent,
    AllTablesComponent,
    SheetComponent,
    DatasetComponent,
    DataSetDialogComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HotTableModule,
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
    MatSelectModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
