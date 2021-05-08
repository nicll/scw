import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import { TableService } from '../Services/table.service';
import { UserService } from '../Services/user.service';


@Component({
  selector: 'app-sheet',
  templateUrl: './sheet.component.html',
  styleUrls: ['./sheet.component.scss']
})
export class SheetComponent implements AfterViewInit, OnInit{

  @Input() tableId:string|undefined;

  constructor(public table:TableService, public user:UserService) { }

  ngAfterViewInit(): void {}
  data= [[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[]];
  cols: any;
  id = 'hotInstance';
  dataset: Array<Array<any>>=new Array;

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
  ngOnInit(): void {
    //++this.user.
  }
  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }
}
