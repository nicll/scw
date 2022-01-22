import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {Log} from "../../Models/Log";
import {UserService} from "../../Services/user.service";
import {Table} from "../../Models/Table";

@Component({
  selector: 'app-show-logs-of-table-dialog',
  templateUrl: './show-logs-of-table-dialog.component.html',
  styleUrls: ['./show-logs-of-table-dialog.component.scss']
})
export class ShowLogsOfTableDialogComponent implements OnInit {

  table: Table;
  logs: Log[];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private userService: UserService) {
    this.logs = new Array<Log>();
    this.table = data;
    console.log(data.tableId)
    let ownedTables
    this.userService.AdminGetAllLogs("User").subscribe((logs: Log[]) => {
      const deletedAllWhereNoTableId = logs.filter(log => log.tableId != null);
        this.logs  = deletedAllWhereNoTableId.filter(log => log.tableId === this.table.tableId)
        console.log(this.logs)
    })
  }

  ngOnInit(): void {
  }

}
