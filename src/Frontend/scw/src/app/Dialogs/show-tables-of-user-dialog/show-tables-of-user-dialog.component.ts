import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog} from "@angular/material/dialog";
import {ApolloService} from "../../Services/apollo.service";
import {User} from "../../Models/User";
import {Table} from "../../Models/Table";
import moment from "moment";
import {Log} from "../../Models/Log";
import {UserService} from "../../Services/user.service";
import {ShowLogsOfTableDialogComponent} from "../show-logs-of-table-dialog/show-logs-of-table-dialog.component";

@Component({
  selector: 'app-show-tables-of-user-dialog',
  templateUrl: './show-tables-of-user-dialog.component.html',
  styleUrls: ['./show-tables-of-user-dialog.component.scss']
})
export class ShowTablesOfUserDialogComponent implements OnInit {

  //cols: any[];
  user: User;
  ownedTables: Table[] = [];
  logs: Log[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public apollo: ApolloService, public userService: UserService, public dialog: MatDialog) {
    //this.cols = this.data.cols;
    this.user = this.data;
    //@ts-ignore
    this.ownedTables = this.user.ownedTables;

    this.userService.AdminGetAllLogs("User").subscribe((logs: Log[]) => {
      const deletedAllWhereNoTableId = logs.filter(log => log.tableId != null);
      for (let i = 0; i < logs.length; i++) {
        this.ownedTables[i].logs = deletedAllWhereNoTableId.filter(log => log.tableId === this.ownedTables[i].tableId)
        console.log(this.ownedTables[i].logs)
      }
    })
  }

  ngOnInit(): void {
    for (let i = 0; i < this.ownedTables.length; i++) {
      console.log("nativ kinda :    " + this.ownedTables[i].creationDate);
      this.ownedTables[i].creationDate = new Date(this.ownedTables[i].creationDate);
    }
  }

  hideTablesDialogAndOpenLogsOfTableDialog(table: Table){
    this.dialog.closeAll();

    this.dialog.open(ShowLogsOfTableDialogComponent, {data: table,       height: '80%',});

    console.log("hideTablesDialogAndOpenLogsOfTableDialog")
  }

}
