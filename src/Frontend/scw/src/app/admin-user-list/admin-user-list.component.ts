import {Component, OnInit, ViewChild} from '@angular/core';
import {UserService} from '../Services/user.service';
import {ApolloService} from '../Services/apollo.service';
import {CollaborationsService} from "../Services/collaborations.service";
import {User} from "../Models/User";
import {Table, TableModule} from 'primeng/table';
import {ConfirmationService, MessageService} from "primeng/api";


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
  users: User[];
  user: User;
  submitted: boolean;
  selectedUsers: any;

  @ViewChild('dt') table: Table | undefined;

  constructor(public userservice: UserService, public apollo: ApolloService, public collab: CollaborationsService,  private messageService: MessageService, private confirmationService: ConfirmationService) {
    this.user = new User("defaultName", "defaultUserId", 0, "defaultRole");
    this.userDialog = false;
    this.submitted = false;
    this.users = []
    this.userservice.GetAllUsersAdmin().subscribe((user: User[]) => {
      this.users = user
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

  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }


  ngOnInit() {
  }

  onActivityChange(event: { target: { value: any; }; }) {
    const value = event.target.value;
    if (value && value.trim().length) {
      const activity = parseInt(value);

      if (!isNaN(activity)) {
        // @ts-ignore
        this.table.filter(activity, 'activity', 'gte');
      }
    }
  }

  /* async ngOnInit() {
     this.users = await this.fetchUsers();
     console.log(this.users)
   }

   async fetchUsers() {
     const cache = this.user.GetAllUsersAdmin();
     console.log(cache);
     return cache;
   }*/

  editUser(user: User) {
    this.user = {...user};
    this.userDialog = true;
    console.log("edit" + this.user.userId)
  }

  deleteUser(user: User) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the user ' + user.name + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.userservice.adminDeleteUser(user.userId).subscribe(() => {
          this.messageService.add({severity: 'success', summary: 'User deleted', detail: 'User ' + user.name + ' deleted'});
        }, error => {
          this.messageService.add({severity: 'error', summary: 'User not deleted', detail: 'User ' + user.name + ' not deleted'});
        })
      }
    });
    console.log("delete " + this.user.userId)
  }

  saveUser() {
    this.submitted = true;
    if (this.user.name.trim()) {
      if (this.user.userId) {
          //TODO add api call

      }
    }
  }

}


