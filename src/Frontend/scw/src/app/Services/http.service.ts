import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import { User } from '../Models/User';
import { HttpErrorHandler, HandleError } from '../http-error-handler.service';
import { Table } from '../Models/Table';
import { DataSet } from '../Models/DataSet';
@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private handleError: HandleError;
  baseUri: string='http://localhost:5000/api';

  constructor(private http:HttpClient,httpErrorHandler: HttpErrorHandler) {
    this.handleError = httpErrorHandler.createHandleError('HeroesService');
  }

  public Login(user:User):Observable<User>{
    return this.http.post<User>(this.baseUri+"/Service/login",user,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('Login',user);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return user;})
    );
  }
  public Signup(user:User):Observable<User>{
    return this.http.post<User>(this.baseUri+"/Service/register",user,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('SignUp',user);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return user;})
    );
  }
  public Logout(){
    return this.http.post(this.baseUri+"/Service/Logout",{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('Logout');
        console.error(err);
        return throwError(err);
      })
    );
  }
  public GetDataSets():Observable<Table[]>{
    return this.http.get<Table[]>(this.baseUri+"/my/dataset",{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetDataSets');
        console.error(err);
        return throwError(err);
      }),
      map(tables=>{return tables;})
    );
  }
  public PostDataSet(table:Table):Observable<Table>{
    return this.http.post<Table>(this.baseUri+"/my/dataset",table,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('PostDataSet');
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return table;})
    );
  }
  public GetSheets():Observable<Table[]>{
    return this.http.get<Table[]>(this.baseUri+"/my/sheet",{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheets');
        console.error(err);
        return throwError(err);
      }),
      map(sheets=>{return sheets;})
    );
  }
  public PostSheet(table:Table):Observable<Table>{
    return this.http.post<Table>(this.baseUri+"/my/sheet",table,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('PostSheet');
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return table;})
    );
  }
  public GetSheet(id:string):Observable<DataSet>{
    return this.http.get<any>(this.baseUri+"/data/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return sheet;})
    );
  }
  public GetDataSet(id:string):Observable<DataSet>{
    return this.http.get<any>(this.baseUri+"/data/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return sheet;})
    );
  }

}
