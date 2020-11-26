import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HandsontestComponent } from './handsontest/handsontest.component';

const routes: Routes = [{path: 'handson',component: HandsontestComponent}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
