import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import { User } from '../Models/User';
import { HttpErrorHandler, HandleError } from '../http-error-handler.service';
import { Table } from '../Models/Table';
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
  public GetDataSet():Observable<Table[]>{
    return this.http.get<Table[]>(this.baseUri+"/my/dataset",{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetDataSet');
        console.error(err);
        return throwError(err);
      }),
      map(user=>{console.log(user); return user;})
    );
  }

}
