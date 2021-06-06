
import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { TableService } from '../Services/table.service';
import { DataSet } from '../Models/DataSet';
import { UserService } from '../Services/user.service';

import {Table} from '../Models/Table';
import { ApolloService } from '../Services/apollo.service';
import { User } from '../Models/User';

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{
  selectedRows: any;

  @Input() tableId:string|undefined;

  constructor(public table:TableService, public user:UserService,public apollo:ApolloService) { }

  ngAfterViewInit(): void {}
  data= [{A:"dataset11",B:"test21",C:"test31",D:"test41"},{A:"dataset12",B:"test2",C:"test3",D:"test4"},{A:"dataset13",B:"test2",C:"test3",D:"test4"},{A:"dataset14",B:"test2",C:"test3",D:"test4"}];
  cols= [{field:"A", header:"A"},{field:"B", header:"B"},{field:"C", header:"C"},{field:"D", header:"D"}];
  dataset: Array<Array<any>>=new Array;
  rowData: any;


  ngOnInit(): void {
    if(this.tableId != undefined){
      let id=this.tableId;
      this.user.GetDataSet(id).subscribe(dataset=>{//Get details of DataSet
        this.apollo.lookUpDataSetId(id).subscribe(id=>{//Get the GraphqlId
          let query=this.apollo.QueryBuilder(id,dataset.columns.map(v=>v.name))//Build our query
          this.apollo.GetData<any>(query).subscribe(data=>{
            this.data=data.data["all"+this.apollo.makeQueryRightCase(id+"s")].nodes;
            this.cols=[];
            dataset.columns.forEach((field)=>
              this.cols=this.cols.concat({field:field.name, header:field.name}))
          });
          //this.apollo.Update("id",12,new Map().set("bruhmode1","200")).subscribe(data=>console.log(data));
        });
      });
    }
  }
  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }

  saveSheet() {

  }

  onKey($event: MouseEvent) {
  }
}

