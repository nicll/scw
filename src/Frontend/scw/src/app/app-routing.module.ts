import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AllTablesComponent } from './all-tables/all-tables.component';
import { HandsontestComponent } from './handsontest/handsontest.component';

const routes: Routes = [{path: 'handson',component: HandsontestComponent},{path:'tables',component:AllTablesComponent}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
