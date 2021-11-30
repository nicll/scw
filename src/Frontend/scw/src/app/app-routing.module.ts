import { AllCollaborationsComponent } from './all-collaborations/all-collaborations.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AllTablesComponent } from './all-tables/all-tables.component';
import { SheetComponent } from './sheet/sheet.component';
import { DatasetComponent } from './dataset/dataset.component';
import {MenubarComponent} from "./menubar/menubar.component";
import {SpreadjsComponent} from "./spreadjs/spreadjs.component";

const routes: Routes =
  [
    {path: 'sheet',component: SheetComponent},
    {path: '',component: DatasetComponent},
    {path:'tables',component:AllTablesComponent},
    {path:'menubarTest',component:MenubarComponent},
    {path:'spreadjs',component:SpreadjsComponent},
    {path:'collabs',component:AllCollaborationsComponent}

  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
