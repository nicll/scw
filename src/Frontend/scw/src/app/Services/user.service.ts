import { Injectable, OnInit } from '@angular/core';
import { User } from '../Models/User';
import { Roles } from '../Models/Roles';
import { Observable, of, pipe, throwError } from 'rxjs';
import {catchError, map} from 'rxjs/operators'
import { HandleError, HttpErrorHandler } from '../http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { Table } from '../Models/Table';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http:HttpClient,httpErrorHandler: HttpErrorHandler) {
    this.handleError = httpErrorHandler.createHandleError('UserService');
  }

  private readonly handleError: HandleError;
  baseUri: string='http://localhost:5000/api';
  user?:User=undefined;

  public GetUser():Observable<User>{
    return this.http.get(this.baseUri+"/my/username",{responseType:'text',withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('Login');
        console.error(err);
        return throwError(err);
      }),
      map(name=>{return this.user=new User(name,"",Roles.Common,"");})
    );
  }
  public Login(username:string,password:string):Observable<User>{
    let us=new User(username,"",Roles.Common,password);
    return this.http.post<User>(this.baseUri+"/Service/login",us,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('Login',us);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{this.user=us;return us;})
    );
  }
  public Signup(username:string,password:string):Observable<User>{
    let user=new User(username,"",Roles.Common,password);
    return this.http.post<User>(this.baseUri+"/Service/register",user,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('SignUp',user);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{this.user=user;return user;})
    );
  }
  public Logout(){
    this.user=undefined;
    return this.http.get(this.baseUri+"/Service/Logout",{withCredentials:true}).subscribe(
        _=>console.log("Logout"));
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
  
  public GetDataSet(id:string):Observable<Table>{
    return this.http.get<Table>(this.baseUri+"/my/dataset/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetDataset');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{console.log(id);return sheet;})
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
  public DeleteDataSet(id:string):Observable<string>{
    return this.http.delete<Table>(this.baseUri+"/my/dataset/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('DeleteDataSet');
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return id;})
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
  
  public GetSheet(id:string):Observable<Table>{
    return this.http.get<Table>(this.baseUri+"/my/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return sheet;})
    );
  }
  public PostSheet(name:string):Observable<string>{
    return this.http.post<Table>(this.baseUri+"/my/sheet",name,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('PostSheet');
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return name;})
    );
  }
  public DeleteSheet(id:string):Observable<string>{
    return this.http.delete<Table>(this.baseUri+"/my/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('DeleteSheet');
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return id;})
    );
  }

}
