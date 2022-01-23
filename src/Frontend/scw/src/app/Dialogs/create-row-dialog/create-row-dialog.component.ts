import {Component, Inject, Input, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import { ApolloService } from '../../Services/apollo.service';

@Component({
  selector: 'app-create-row-dialog',
  templateUrl: './create-row-dialog.component.html',
  styleUrls: ['./create-row-dialog.component.scss']
})
export class CreateRowDialogComponent implements OnInit {

  cols: any[];
  answer: any[] = [];
  tableId: string;
  mapData: Map<string, string> | undefined

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public apollo: ApolloService) {
    console.log(this.data.cols);
    this.cols = this.data.cols;
    this.tableId = this.data.tableId;


  }

  ngOnInit(): void {
    for (let i = 0; i < this.cols.length; i++) {
      console.log(this.cols.length);
      console.log(  document.getElementsByName("killMe"));
      document.getElementsByName("killMe")[i].id = "killMe" + i;
    }
  }

  addRow() {
    for (let i = 0; i < this.cols.length; i++) {
      //console.log(document.getElementsByName("killMe")[i].id);
    }
    for (let i = 0; i < this.cols.length; i++) {
      // @ts-ignore
      this.answer[i] = document.getElementById("mat-input-" + i).value;
    }
    console.log(this.answer);

    this.mapData = new Map<string, string>();
    for (let i = 0; i < this.cols.length; i++) {
      this.mapData.set(this.cols[i].field, this.answer[i]);
    }
    this.apollo.lookUpDataSetId(this.tableId).subscribe(z => {
      console.log(this.mapData);
      this.apollo.Insert(z, this.mapData!).subscribe();
    } );

    }
  }
