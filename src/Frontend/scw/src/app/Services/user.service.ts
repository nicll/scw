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
@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {
  }

  baseUri: string = environment.aspUri+'/api';
  username: string = "";

  //Basic user api

  public GetUserName(): Observable<string> {
    return this.http
      .get(this.baseUri + '/my/username', {
        responseType: 'text',
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          return throwError(err);
        }),
        map((name) => {
          return this.username=name;
        })
      );
  }
  public Login(username: string, password: string): Observable<any> {
    let us = {"username":username, "password":new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)))};
    return this.http
      .post<User>(this.baseUri + '/Service/login', us, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          return throwError(err);
        }),
        map((_) => {
          console.log("login successful")
          this.username=us.username;
          return us;
        })
      );
  }
  public Signup(username: string, password: string): Observable<any> {
    let user = {"username":username, "password":new TextDecoder("utf-8").decode(sha256(new TextEncoder().encode(password)))};
    return this.http
      .post<User>(this.baseUri + '/Service/register', user, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          return throwError(err);
        }),
        map((_) => {
          this.username = username;
          return user;
        })
      );
  }
  public Logout() {
    this.username = "";
    return this.http
      .get(this.baseUri + '/Service/Logout', { withCredentials: true })
      .subscribe((_) => console.log('Logout'));
  }

  //basic DataSet api

  public GetDataSets(): Observable<Table[]> {
    return this.http
      .get<Table[]>(this.baseUri + '/my/dataset', { withCredentials: true })
      .pipe(
        catchError((err) => {
          console.error(err);
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
          return throwError(err);
        }),
        map((sheet) => {
          return sheet;
        })
      );
  }

}
