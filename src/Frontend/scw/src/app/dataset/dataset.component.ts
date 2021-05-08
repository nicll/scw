
import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { TableService } from '../Services/table.service';
import { DataSet } from '../Models/DataSet';
import { UserService } from '../Services/user.service';

import {Table} from '../Models/Table';

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{

  @Input() tableId:string|undefined;

  constructor(public table:TableService, public user:UserService) { }
  data:Table[]= [
    {
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "displayName": "DatasetName1",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    },{
      "tableRefId": "3fa85f64-5717-4562-b3fc-2c963f66afa2",
      "displayName": "DatasetName2",
      "tableType": "DataSet",
      "ownerUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "columns": [
        {
          "position": 0,
          "name": "string",
          "type": "Integer",
          "nullable": true
        }
      ]
    }];
  namesOfTables = [];
  cols= ["Col1","Col2","Col3","Col4","Col5"];
  id = 'hotInstance';
  dataset: Array<Array<any>>=new Array;

  ngAfterViewInit(): void { }
  ngOnInit(): void {
    console.log(this.data[0]);
  }


  public saveSheet(){

  }
  public onKey(event: any) { // without type info
    // @ts-ignore
    let exportPlugin1 = this.hot.getPlugin('exportFile');
    exportPlugin1.downloadFile('csv', {
      bom: false,
      columnDelimiter: ',',
      columnHeaders: false,
      exportHiddenColumns: true,
      exportHiddenRows: true,
      fileExtension: 'csv',
      filename: 'Handsontable-csv-file_[YYYY]-[MM]-[DD]',
      mimeType: 'text/csv',
      rowDelimiter: '\r\n',
      rowHeaders: true
    });
  };

  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }
}
