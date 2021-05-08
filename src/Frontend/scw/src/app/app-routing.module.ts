import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AllTablesComponent } from './all-tables/all-tables.component';
import { HandsontestComponent } from './handsontest/handsontest.component';
import { SheetComponent } from './sheet/sheet.component';
import { DatasetComponent } from './dataset/dataset.component';
import {HotTableModule} from "@handsontable/angular";
import {MenubarComponent} from "./menubar/menubar.component";

const routes: Routes =
  [{path: 'handson',component: HandsontestComponent},
    {path: 'sheet',component: SheetComponent},
    {path: '',component: DatasetComponent},
    {path:'tables',component:AllTablesComponent},
    {path:'menubarTest',component:MenubarComponent}
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes),HotTableModule.forRoot()],
  exports: [RouterModule]
})
export class AppRoutingModule { }
