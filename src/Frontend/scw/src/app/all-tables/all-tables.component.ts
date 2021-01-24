import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { HttpService } from '../Services/http.service';

@Component({
  selector: 'app-all-tables',
  templateUrl: './all-tables.component.html',
  styleUrls: ['./all-tables.component.scss']
})
export class AllTablesComponent implements OnInit {
  tabs:any;
  constructor(private http:HttpService) { }

  ngOnInit(): void {
    //this.http.GetDataSet().subscribe(tables=>this.tabs=tables);//For test purposes
    this.tabs=[{displayName:"1"},{displayName:"2"}];
    console.log(this.tabs);
    this.http.GetSheet("7cc78b45-9489-4610-9567-e795374dfa04").subscribe(v=>console.log(v));
  }

}
