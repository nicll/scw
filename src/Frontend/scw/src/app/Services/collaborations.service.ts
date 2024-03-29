import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from './../../environments/environment';
import { User } from './../Models/User';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, pipe, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Table } from '../Models/Table';

@Injectable({
  providedIn: 'root'
})
export class CollaborationsService {

  private baseUri: string = environment.aspUri+'/api';

  constructor(private http: HttpClient, private snackbar: MatSnackBar) {
  }
  public AddCollaborator(tableid: string, userid: string): Observable<void> {
    console.log(this.baseUri + '/my/table/'+tableid+"/collaborator/"+userid)
    return this.http
      .post(this.baseUri + '/my/table/'+tableid+"/collaborator/"+userid,"test",{
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((_) => {
        })
      );
  }
  public GetCollabs(): Observable<Table[]> {
    return this.http
      .get<Table[]>(this.baseUri + '/my/table/collaborations', { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((tables) => {
          return tables;
        })
      );
  }
  public GetCollabsForTable(tableId:string): Observable<User[]> {
    return this.http
      .get<User[]>(this.baseUri + '/my/table/' + tableId+'/collaborator', { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((users) => {
          return users;
        })
      );
  }
  public RemoveCollabs(tableid: string, userid: string): Observable<void> {
    return this.http
      .delete(this.baseUri + '/my/table/'+tableid+"/collaborator/"+userid,{
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((_) => {
        })
      );
  }
}
