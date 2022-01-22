import { Component, OnInit } from '@angular/core';
import {ChartModule} from 'primeng/chart';
import { UserService } from '../Services/user.service';
import {User} from "../Models/User";
import {Table as TableModel, Table} from "../Models/Table";
import {StatsMapRolesToCount} from "../Models/StatsMapRolesToCount";

@Component({
  selector: 'app-admin-statistics-page',
  templateUrl: './admin-statistics-page.component.html',
  styleUrls: ['./admin-statistics-page.component.scss']
})
export class AdminStatisticsPageComponent{
  basicData: any;
  stackedData: any;
  stackedOptions: any;
  multiAxisData: any;
  multiAxisOptions: any;
  lineStylesData: any;
  basicOptions: any;
  dataGroupedByMonth: Map<number, number>;
  values: number[];
  tables?: Table[];
  users?: User[];
  commonCounts : number[];
  managerCounts : number[];
  adminCounts : number[];
  mapUserRolesToLastModifiedAndCount: StatsMapRolesToCount[];

 constructor(public user: UserService) {
   this.adminCounts = [0,0,0,0,0,0,0,0,0,0,0,0];
   this.commonCounts = [0,0,0,0,0,0,0,0,0,0,0,0];
   this.managerCounts = [0,0,0,0,0,0,0,0,0,0,0,0];
    this.users = new Array<User>();
    this.tables = new Array<Table>();
    this.mapUserRolesToLastModifiedAndCount = Array<StatsMapRolesToCount>();
    let commonShit = new StatsMapRolesToCount("common", this.commonCounts);
    let managerShit = new StatsMapRolesToCount("manager", this.managerCounts);
    let adminShit = new StatsMapRolesToCount("admin", this.adminCounts);
    this.mapUserRolesToLastModifiedAndCount.push(commonShit);
    this.mapUserRolesToLastModifiedAndCount.push(managerShit);
    this.mapUserRolesToLastModifiedAndCount.push(adminShit);
    this.dataGroupedByMonth = new Map<number, number>();
    this.values = [];
    this.user.GetAllUsersAdmin().subscribe(users => {
        this.users = users;
        console.log(this.users);

      console.log(this.users + "  vor schleife#####")
      for (let i = 0; i < this.users.length; i++) {
        console.log("user schleife  " + this.users[i].role);
        this.user.AdminGetTablesOfUser(this.users[i].userId).subscribe((tables: Table[]) => {
          for (let j = 0; j < tables.length; j++) {
            const cacheDateOfTable = new Date(tables[j].creationDate).getMonth();
            console.log("cacheDateOfTable " + cacheDateOfTable);
            // @ts-ignore
            console.log("USERROLE ####" + this.users[i].role);
            // @ts-ignore
            if(this.users[i].role.toString() == "Common") {
              this.commonCounts[cacheDateOfTable] = this.commonCounts[cacheDateOfTable] + 1;
              this.mapUserRolesToLastModifiedAndCount[0]= new StatsMapRolesToCount("common", this.commonCounts);
            }
            // @ts-ignore
            if(this.users[i].role.toString() == "Manager") {
              this.managerCounts[cacheDateOfTable] = this.managerCounts[cacheDateOfTable] + 1;
              this.mapUserRolesToLastModifiedAndCount[0]= new StatsMapRolesToCount("manager", this.managerCounts);

            }
            // @ts-ignore
            if(this.users[i].role.toString() == "Admin") {
              this.adminCounts[cacheDateOfTable] = this.adminCounts[cacheDateOfTable] + 1;
              this.mapUserRolesToLastModifiedAndCount[0]= new StatsMapRolesToCount("admin", this.adminCounts);
            }

          }


        })

      }
    });
    this.user.AdminGetTablesOfAllUsers().subscribe(data => {
      this.tables = data;
      console.log(data);
      for (let i = 0; i < data.length; i++) {
        let date = new Date(data[i].creationDate);
        //console.log(date.getMonth());
        if(this.dataGroupedByMonth.get(date.getMonth()+1) == undefined) {
          this.dataGroupedByMonth.set(date.getMonth()+1, 1);
        } else {
          // @ts-ignore
          this.dataGroupedByMonth.set(date.getMonth()+1, this.dataGroupedByMonth.get(date.getMonth()+1) + 1);
        }
      }
     // console.log(this.dataGroupedByMonth)
      this.values = Array.from(this.dataGroupedByMonth.values())
      //console.log("values" + this.values);
      //console.log(typeof this.values)
    });

    for (let i = 1; i < 13; i++) {
      if(this.dataGroupedByMonth.get(i) == undefined) {
        this.dataGroupedByMonth.set(i, 0);
      }
    }


    //console.log("values" + this.values);
    setTimeout(() => {
      this.basicData = {
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datasets: [
          {
            label: 'Tables created by all Users',
            data: this.values,
            fill: false,
            borderColor: '#42A5F5',
            tension: .4
          },
          {
            label: 'Tables created by all Commons',
            data: this.commonCounts,
            fill: false,
            borderColor: '#FFA726',
            tension: .4
          },
          {
            label: 'Tables created by all Managers',
            data: this.managerCounts,
            fill: false,
            borderColor: '#32a86f',
            tension: .4
          },
          {
            label: 'Tables created by all Admins',
            data: this.adminCounts,
            fill: false,
            borderColor: '#9a32a8',
            tension: .4
          }
        ]
      };
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
    }; }, 700);
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


    this.basicOptions = {
      plugins: {
        legend: {
          labels: {
            color: '#ebedef'
          }
        }
      },
      scales: {
        x: {
          ticks: {
            color: '#ebedef'
          },
          grid: {
            color: 'rgba(255,255,255,0.2)'
          }
        },
        y: {
          ticks: {
            color: '#ebedef'
          },
          grid: {
            color: 'rgba(255,255,255,0.2)'
          }
        }}};}



      selectData(event: any) {
        console.log(event.element);
      }

}


