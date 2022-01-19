import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {ApolloService} from "../../Services/apollo.service";
import {User} from "../../Models/User";
import {Table} from "../../Models/Table";
import moment from "moment";

@Component({
  selector: 'app-show-tables-of-user-dialog',
  templateUrl: './show-tables-of-user-dialog.component.html',
  styleUrls: ['./show-tables-of-user-dialog.component.scss']
})
export class ShowTablesOfUserDialogComponent implements OnInit {

  //cols: any[];
  user: User;
  ownedTables: Table[] = [];


  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public apollo: ApolloService) {
    //this.cols = this.data.cols;
    this.user = this.data;
    //@ts-ignore
    this.ownedTables = this.user.ownedTables;

  }

  ngOnInit(): void {
    for (let i = 0; i < this.ownedTables.length; i++) {
      console.log("nativ kinda :    " + this.ownedTables[i].creationDate);
      this.ownedTables[i].creationDate = new Date(this.ownedTables[i].creationDate);

      /*let cacheDate = new Date(this.ownedTables[i].creationDate);
      console.log("before  " + cacheDate);
      //cacheDate =  new Date(moment(cacheDate).format('DD-MM-YYYY'));
      console.log(moment(this.ownedTables[i].creationDate,'YYYY-MM-DD hh:mm:ss a',  false).format('DD/MM/YYYY'));
      console.log("after  " + cacheDate);
      this.ownedTables[i].creationDate = new Date(moment(this.ownedTables[i].creationDate,'YYYY-MM-DD hh:mm:ss a',  false).format('MM/DD/YYYY'));*/
      //console.log(cacheDate.toISOString().split('T')[0]);
      //this.ownedTables[i].creationDate = new Date(this.ownedTables[i].creationDate.toISOString().split('T')[0]);
    }
  }

}
