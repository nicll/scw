import { Injectable, OnInit } from '@angular/core';
import { User } from '../Models/User';
import { Roles } from '../Models/Roles';
import { Observable, of, pipe, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Table } from '../Models/Table';
import { Column } from '../Models/Column';
import sha256 from 'fast-sha256';
import { environment } from 'src/environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient, private snackbar: MatSnackBar) {
  }

  baseUri: string = environment.aspUri+'/api';
  username: string = "";

  httpTextConfig:{ responseType:"text", withCredentials:boolean} =
  { responseType: 'text', withCredentials: true}

  httpConfig = { withCredentials: true}

  //Basic user api
  private OnError(error:Error):string{
    console.error(error);
    this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
    return "Error in communication with backend";
  }
  public GetUserName(): Observable<string|undefined> {
    if(this.username)
      return of(this.username);
    else
      return this.http.get(this.baseUri + '/my/username', this.httpTextConfig).pipe(
        catchError(err => this.OnError(err)),
        map(name => this.username = name)
      );
  }
  public Login(username: string, password: string): Observable<User | undefined> {
    let us:User = {
      "username":username,
      "password":new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)))
    };

    return this.http.post<User>(this.baseUri + '/Service/login', us, this.httpConfig).pipe(
      catchError(err => this.OnError(err)),
      map((_) => {
          this.snackbar.open("Successful login", undefined, {duration:5000});
          this.username = us.username;
          return us;
        }),

      );
  }
  public Signup(username: string, password: string): Observable<any> {
    let user:User = {
      "username":username,
      "password":new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)))
    };
    return this.http.post<User>(this.baseUri + '/Service/register', user, this.httpConfig).pipe(
        catchError((err) => this.OnError(err)),
        map((_) => {
          this.snackbar.open("Successful SignUp", undefined, {duration:5000});
          this.username = username;
          return user;
        })
      );
  }
  public Logout() {
    this.username = "";
    return this.http
      .get(this.baseUri + '/Service/Logout', { withCredentials: true })
      .subscribe(
        (_) => this.snackbar.open("Successful SignUp", undefined, {duration:5000}),
        (error) => this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
      );
  }

  //basic DataSet api

  public GetDataSets(): Observable<Table[]> {
    return this.http
      .get<Table[]>(this.baseUri + '/my/dataset', { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((tables) => {
          return tables;
        })
      );
  }
  public GetDataSet(id: string): Observable<Table> {
    return this.http
      .get<Table>(this.baseUri + '/my/dataset/' + id, { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((sheet) => {
          console.log(id);
          return sheet;
        })
      );
  }
  public PostDataSet(table: Table): Observable<Table> {
    return this.http
      .post<Table>(this.baseUri + '/my/dataset', table, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return table;
        })
      );
  }
  public DeleteDataSet(id: string): Observable<string> {
    return this.http
      .delete<Table>(this.baseUri + '/my/dataset/' + id, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return id;
        })
      );
  }

  //basic Sheet api

  public GetSheets(): Observable<Table[]> {
    return this.http
      .get<Table[]>(this.baseUri + '/my/sheet', { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((sheets) => {
          return sheets;
        })
      );
  }
  public GetSheet(id: string): Observable<Table> {
    return this.http
      .get<Table>(this.baseUri + '/my/sheet/' + id, { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((sheet) => {
          return sheet;
        })
      );
  }
  public PostSheet(name: string): Observable<string> {
    return this.http
      .post<Table>(this.baseUri + '/my/sheet', name, { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return name;
        })
      );
  }
  public DeleteSheet(id: string): Observable<string> {
    return this.http
      .delete<Table>(this.baseUri + '/my/sheet/' + id, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return id;
        })
      );
  }

  //Column add/delete

  public PostColumn(
    id: string,
    name: string,
    column: Column
  ): Observable<string> {
    return this.http
      .post<Table>(this.baseUri + '/my/dataset/' + id + '/' + name, column, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return id;
        })
      );
  }
  public DeleteColumn(id: string, name: string): Observable<string> {
    return this.http
      .delete<Table>(this.baseUri + '/my/dataset/' + id + '/' + name, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((_) => {
          return id;
        })
      );
  }
  //Conversion userid <-> username
  public GetUserId(username:string):Observable<string>{
    return this.http
      .get(this.baseUri + '/Map/name2id/'+username, { responseType: 'text', withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
        map((sheet) => {
          return sheet.replace('"','').replace('"','');
        })
      );
  }

  public  adminDeleteUser(userId: string) {
    return this.http
      .delete<Table>(this.baseUri + '/Admin/user/' + userId, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000});
          return throwError(err);
        }),
      );
  }
}

