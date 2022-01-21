import { AllCollaborationsComponent } from './all-collaborations/all-collaborations.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AllTablesComponent } from './all-tables/all-tables.component';
import { SheetComponent } from './sheet/sheet.component';
import { DatasetComponent } from './dataset/dataset.component';
import {MenubarComponent} from "./menubar/menubar.component";
import {SpreadjsComponent} from "./spreadjs/spreadjs.component";
import {AdminUserListComponent} from "./admin-user-list/admin-user-list.component";
import {AdminStatisticsPageComponent} from "./admin-statistics-page/admin-statistics-page.component";
import {LoginGuard} from "./Services/login.guard";

const routes: Routes =
  [
    {path: 'sheet',component: SheetComponent,   canActivate: [LoginGuard]},
    {path: '',component: DatasetComponent},
    {path:'tables',component:AllTablesComponent,   canActivate: [LoginGuard]},
    {path:'menubarTest',component:MenubarComponent,   canActivate: [LoginGuard]},
    {path:'collabs',component:AllCollaborationsComponent,   canActivate: [LoginGuard]},
    {path:'upload',component:MenubarComponent,   canActivate: [LoginGuard]},
    {path:'admin',component:AdminUserListComponent,   canActivate: [LoginGuard]},
    {path:'stats',component:AdminStatisticsPageComponent,   canActivate: [LoginGuard]},
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
