import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HandsontestComponent } from './handsontest/handsontest.component';
import { HotTableModule } from '@handsontable/angular';
import { LogInSignUpDialogComponent } from './log-in-sign-up-dialog/log-in-sign-up-dialog.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatButtonModule} from '@angular/material/button';
import {HttpClientModule} from '@angular/common/http'

@NgModule({
  declarations: [
    AppComponent,
    HandsontestComponent,
    LogInSignUpDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HotTableModule,
    BrowserAnimationsModule,
    MatButtonModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
