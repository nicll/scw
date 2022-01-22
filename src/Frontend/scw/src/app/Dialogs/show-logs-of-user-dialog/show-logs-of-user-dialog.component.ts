import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {UserService} from "../../Services/user.service";
import {Log} from "../../Models/Log";
import {User} from "../../Models/User";

@Component({
  selector: 'app-show-logs-of-user-dialog',
  templateUrl: './show-logs-of-user-dialog.component.html',
  styleUrls: ['./show-logs-of-user-dialog.component.scss']
})
export class ShowLogsOfUserDialogComponent implements OnInit {

  user : User;
  logs : Log[];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private userService: UserService) {
    this.user = data;
    this.logs = new Array<Log>();
    console.log(data.tableId)
    let ownedTables
    this.userService.AdminGetAllLogs("User").subscribe((logs: Log[]) => {
      const deletedAllWhereNoTableId = logs.filter(log => log.userId != null);
      this.logs = deletedAllWhereNoTableId.filter(log => log.userId === this.user.userId)
      console.log(this.logs)
    })
  }

  ngOnInit(): void {
  }

}
