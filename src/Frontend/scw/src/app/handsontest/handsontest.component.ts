import { Component, OnInit } from '@angular/core';
import { HttpService } from '../Services/http.service';

@Component({
  selector: 'app-handsontest',
  templateUrl: './handsontest.component.html',
  styleUrls: ['./handsontest.component.scss']
})
export class HandsontestComponent implements OnInit {

  constructor(private http:HttpService) { }
  dataset: any[]=new Array;
  tables: string[] = ['T1','T2','T3'];
  ngOnInit(): void {
    this.dataset=[['','Row1','Row2','row3'],["testdata","row2","row3"],[1,2,3],[3,2,1]];
    console.log("test");
    this.http.GetDataSets().subscribe(v=>console.log(v));
  }

}
