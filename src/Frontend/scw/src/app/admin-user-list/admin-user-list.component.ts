import {Component, OnInit, ViewChild} from '@angular/core';
import {UserService} from '../Services/user.service';
import {ApolloService} from '../Services/apollo.service';
import {CollaborationsService} from "../Services/collaborations.service";
import {User} from "../Models/User";
import {Table, TableModule} from 'primeng/table';
import {ConfirmationService, MenuItem, MessageService} from "primeng/api";
import GC from "@grapecity/spread-sheets";
import Tables = GC.Spread.Sheets.Tables;
import {DataSet} from "../Models/DataSet";
import {MatDialog} from "@angular/material/dialog";
import {AddColumnDialogComponent} from "../Dialogs/add-column-dialog/add-column-dialog.component";
import {
  ShowTablesOfUserDialogComponent
} from "../Dialogs/show-tables-of-user-dialog/show-tables-of-user-dialog.component";


@Component({
  selector: 'app-admin-user-list',
  providers: [UserService, ApolloService, ConfirmationService, MessageService],
  templateUrl: './admin-user-list.component.html',
  styles: [`
    :host ::ng-deep .p-dialog .user-image {
      width: 150px;
      margin: 0 auto 2rem auto;
      display: block;
    }
  `],
  styleUrls: ['./admin-user-list.component.scss']
})
export class AdminUserListComponent implements OnInit {
  userDialog: boolean;
  viewUserDialog: boolean;
  users: User[];
  user: User;
  submitted: boolean;
  selectedUsers: any;
  selectedUser: User;
  items: MenuItem[];

  @ViewChild('dt') table: Table | undefined;

  constructor(public dialog: MatDialog,public userservice: UserService, public apollo: ApolloService, public collab: CollaborationsService, private messageService: MessageService, private confirmationService: ConfirmationService) {
    this.selectedUser = new User("defaultName", "defaultUserId", 0, "defaultRole");
    this.items = [
      {label: 'View Details', icon: 'pi pi-fw pi-search', command: () => this.viewUser(this.selectedUser)},
      {label: 'Feature2', icon: 'pi pi-fw pi-times'}
    ];
    this.user = new User("defaultName", "defaultUserId", 0, "defaultRole");
    this.userDialog = false;
    this.viewUserDialog = false;
    this.submitted = false;
    this.users = []

    this.userservice.GetAllUsersAdmin().subscribe((usersTransmitted: string|User[]) => {
      this.users = usersTransmitted as User[]
      for (let i = 0; i < this.users.length; i++) {
        // @ts-ignore
        this.userservice.AdminGetTablesOfUser(this.users[i].userId).subscribe((tables: Table[]) => {
          // @ts-ignore
          this.users[i].ownedTables = tables
        })
        }
      console.log(this.users);
    }, error => {
      console.log(error)
    })
  }

  openNew() {
    //this.user = new User("defaultName", "defaultUserId", 0, "defaultRole");
    this.submitted = false;
    this.userDialog = true;
  }

  viewUser(user: User) {
    this.userservice.AdminGetTablesOfUser(user.userId!).subscribe(tables=> {
      this.userservice.AdminGetCollabsOfUser(user.userId!).subscribe(collabs => {
        //this.viewUserDialog = true;
        this.messageService.add({
          severity: 'info',
          summary: 'User Selected',
          detail: String("this User owns " + tables.length + " tables and collaborates on " + collabs.length + " tables")
        });
      })
    })
  }
  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }
  ngOnInit() {
  }
  editUser(user: User) {
    this.user = {...user};
    this.userDialog = true;
    console.log("edit" + this.user.userId)
  }

  deleteUser(user: User) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the user ' + user.username + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.userservice.adminDeleteUser(user.userId!).subscribe(() => {
          this.messageService.add({
            severity: 'success',
            summary: 'User deleted',
            detail: 'User ' + user.username + ' deleted'
          });
        }, error => {
          this.messageService.add({
            severity: 'error',
            summary: 'User not deleted',
            detail: 'User ' + user.username + ' not deleted'
          });
        })
      }
    });
    console.log("delete " + this.user.userId)
  }

  saveUser() {
    this.submitted = true;
    if (this.user.username.trim()) {
      if (this.user.userId) {
        console.log(this.user.userId)
      }
    }
  }

  showTablesOfUserDialog(user: User) {
    console.log(user)
    const dialogRef = this.dialog.open(ShowTablesOfUserDialogComponent, {
      data: user,
    });
      console.log("showTablesOfUserDialog")
  }
}


