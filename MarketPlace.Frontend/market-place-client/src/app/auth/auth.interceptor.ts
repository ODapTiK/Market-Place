import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { AuthService } from "./auth.service";
import { inject } from "@angular/core";
import { catchError, switchMap, throwError } from "rxjs";

let isRefreshing = false;

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
    const authService = inject(AuthService);

    const token = authService.cookieService.get("access_token");

    if(!token) return next(request);    

    if(isRefreshing) {
        return refreshAndProceed(authService, request, next);
    }

    return next(addToken(request, token))
        .pipe(
            catchError(error => {
                if(error.status === 401) {
                    return refreshAndProceed(authService, request, next);
                }

                return throwError(error);
            })
        );
}

const refreshAndProceed = (authService: AuthService, 
                           request: HttpRequest<any>, 
                           next: HttpHandlerFn) => {
    if(!isRefreshing) {
        isRefreshing = true;
        return authService.refreshAccessToken()
        .pipe(
            switchMap(tokens => {
                isRefreshing = false;
                return next(addToken(request, tokens.accessToken));
            })
        );
    }
    
    return next(addToken(request, authService.cookieService.get("access_token")));
}

const addToken = (request: HttpRequest<any>, token: string) => {
    return request.clone({
        setHeaders:{
            Authorization: `Bearer ${token}`
        }
    })
}