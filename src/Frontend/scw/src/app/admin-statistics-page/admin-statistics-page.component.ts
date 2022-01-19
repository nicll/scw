import { Component, OnInit } from '@angular/core';
import {ChartModule} from 'primeng/chart';
import { UserService } from '../Services/user.service';

@Component({
  selector: 'app-admin-statistics-page',
  templateUrl: './admin-statistics-page.component.html',
  styleUrls: ['./admin-statistics-page.component.scss']
})
export class AdminStatisticsPageComponent implements OnInit {
  basicData: any;
  stackedData: any;
  stackedOptions: any;

  dataGroupedByMonth: Map<number, number>;
  values: number[];

  constructor(public user: UserService) {
    this.dataGroupedByMonth = new Map<number, number>();
    this.values = [];
    this.user.AdminGetTablesOfAllUsers().subscribe(data => {
      console.log(data);
      for (let i = 0; i < data.length; i++) {

        let date = new Date(data[i].creationDate);
        console.log(date.getMonth());
        if(this.dataGroupedByMonth.get(date.getMonth()+1) == undefined) {
          this.dataGroupedByMonth.set(date.getMonth()+1, 1);
        } else {
          // @ts-ignore
          this.dataGroupedByMonth.set(date.getMonth()+1, this.dataGroupedByMonth.get(date.getMonth()+1) + 1);
        }
      }
      console.log(this.dataGroupedByMonth)
      this.values = Array.from(this.dataGroupedByMonth.values())
      console.log("values" + this.values);
      console.log(typeof this.values)
    });

    for (let i = 1; i < 13; i++) {
      if(this.dataGroupedByMonth.get(i) == undefined) {
        this.dataGroupedByMonth.set(i, 0);
      }
    }

    console.log("values" + this.values);
    setTimeout(() => {
    this.stackedData = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
      datasets: [
        {
        type: 'bar',
        label: 'All Users',
        backgroundColor: '#42A5F5',
        data: this.values
      }/*, {
        type: 'bar',
        label: 'Dataset 2',
        backgroundColor: '#66BB6A',
        data: [
          21,
          84,
          24,
          75,
          37,
          65,
          34
        ]
      }, {
        type: 'bar',
        label: 'Dataset 3',
        backgroundColor: '#FFA726',
        data: [
          41,
          52,
          24,
          74,
          23,
          21,
          32
        ]
      }*/]
    }; }, 1000);


    this.stackedOptions = {
      tooltips: {
        mode: 'index',
        intersect: false
      },
      responsive: true,
      scales: {
        xAxes: [{
          stacked: true,
        }],
        yAxes: [{
          stacked: true
        }]
      }
    };

    this.updateChartOptions();



  }

  ngOnInit(): void {

  }

  updateChartOptions() {
  }

  selectData(event : any)  {
    console.log(event.element);
  }
}

