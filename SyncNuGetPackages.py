import os
import shutil

# Folder containing NuGet packages
nuget_folder = "./Packages"
# Folder to collect the preferred DLLs
preferred_dlls_folder = "./Assets/Plugins/SyncedNuGetDLLs"

# list of frameworks to check in order
check_frameworks = ["netstandard2.1", "netstandard2.0", "netstandard1.3", "netstandard1.1", "net472", "net462", "net461", "net46", "net45", "netcoreapp2.1", "netcoreapp2.0"]

for root, dirs, files in os.walk(nuget_folder):
    for dir in dirs:
        if dir.startswith("lib"):
            lib_folder = os.path.join(root, dir)
            package_name = os.path.basename(root)
            found_match = False
            for framework in check_frameworks:
                if framework in os.listdir(lib_folder):
                    for file in os.listdir(
                            os.path.join(lib_folder, framework)):
                        if file.endswith(".dll"):
                            shutil.copy(
                                os.path.join(
                                    lib_folder,
                                    framework,
                                    file),
                                preferred_dlls_folder)
                            print(
                                f"{framework} | {package_name}")
                            found_match = True
                            break
                    if found_match:
                        break
            if found_match:
                continue
            if not found_match:
                print(f"\nError: Preferred framework not found for package {package_name}.")
                print("Available frameworks in lib folder:", os.listdir(lib_folder))
                print("\n")

